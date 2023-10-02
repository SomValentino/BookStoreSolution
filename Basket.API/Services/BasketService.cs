using AutoMapper;
using Basket.API.Models;
using Basket.API.Models.ShoppingCartAggregate;
using Basket.API.Repository;
using EventBus.Messages.Events;
using MassTransit;
using Newtonsoft.Json;
using Polly;

namespace Basket.API.Services;

public class BasketService : IBasketService {
    private readonly IBasketRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<BasketService> _logger;

    public BasketService (IBasketRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        ILogger<BasketService> logger) {
        _repository = repository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    public async Task CheckoutAsync (string userId, string username, string firstName,
        string lastName, string emailAddress, ShoppingCart shoppingCart) {

        var basketCheckout = new BasketCheckout {
            UserName = username,
            TotalPrice = shoppingCart.TotalPrice,
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = emailAddress,
            Items = shoppingCart.Items
        };
        // send checkout event to rabbitmq
        var eventMessage = _mapper.Map<BasketCheckoutEvent> (basketCheckout);

        _logger.LogInformation("Sending order to eventbus:{order}", JsonConvert.SerializeObject (eventMessage));

        await Policy.Handle<Exception> ().WaitAndRetryAsync (5,
                retryAttempt => TimeSpan.FromSeconds (Math.Pow (2, retryAttempt)),
                (exception, timespan, context) => {
                    _logger.LogError ("Error in publishing payload: {payload} to eventbus", JsonConvert.SerializeObject (eventMessage));

                    _logger.LogInformation ("Retrying to publish payload in {timespan}", timespan);
                })
            .ExecuteAsync (() => _publishEndpoint.Publish (eventMessage));
        
        _logger.LogInformation ("Sent order to eventbus");

        // remove the basket
        await _repository.DeleteBasket (userId);
    }

    public async Task<ShoppingCart> GetBasketAsync (string UserId) {
        var basket = await _repository.GetBasket (UserId);

        return basket ?? new ShoppingCart (UserId);
    }

    public async Task<ShoppingCart?> UpdateBasetAsync (string UserId, IEnumerable<ShoppingCartItem> items) {
        var basket = new ShoppingCart (UserId);
        basket.Items.AddRange (items);

        return await _repository.UpdateBasket (basket);
    }
}