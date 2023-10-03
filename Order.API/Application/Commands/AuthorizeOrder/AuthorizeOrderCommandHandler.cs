using Ardalis.GuardClauses;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using MongoDB.Driver;
using Newtonsoft.Json;
using Order.API.Data.Repository.Interfaces;
using Order.API.GrpcClient;
using Order.API.Models;
using Polly;

namespace Order.API.Application.Commands.AuthorizeOrder;

public class AuthorizeOrderCommandHandler : IRequestHandler<AuthorizeOrderCommand, AuthorizeOrderViewDto> {
    private readonly IOrderRepository _orderRepository;
    private readonly PaymentGrpcClientService _paymentGrpcClientService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<AuthorizeOrderCommandHandler> _logger;

    public AuthorizeOrderCommandHandler (IOrderRepository orderRepository,
        PaymentGrpcClientService paymentGrpcClientService,
        IPublishEndpoint publishEndpoint,
        ILogger<AuthorizeOrderCommandHandler> logger) {
        _orderRepository = orderRepository;
        _paymentGrpcClientService = paymentGrpcClientService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    public async Task<AuthorizeOrderViewDto> Handle (AuthorizeOrderCommand request, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation ("Retreiving order from database");

            Models.Order order = await _orderRepository.GetOrderById (request.OrderId);

            Guard.Against.Null (order, nameof (order));

            if (order.OrderStatus != OrderStatus.Pending) {
                return new AuthorizeOrderViewDto {
                    OrderId = request.OrderId,
                    OrderStatus = order.OrderStatus,
                    ErrorMessage = "Only pending orders can be authorized for payment"
                };
            }

            _logger.LogInformation ("Retrieved order of id {id} from database", order.OrderId);

            _logger.LogInformation ("Sending payment authorization to PurchaseToken API for userId {user} and amount {amount} BookPurchaseTokens",
                order.UserId, order.TotalPrice);

            var paymentResponse = await _paymentGrpcClientService.Authorize (new Protos.PaymentRequest {
                UserId = order.UserId,
                Amount = order.TotalPrice
            });

            if (!paymentResponse.Status) {

                order.OrderStatus = OrderStatus.Failed;
                await _orderRepository.UpdateOrder (order);

                return new AuthorizeOrderViewDto {
                    OrderId = order.OrderId,
                    OrderStatus = order.OrderStatus,
                    ErrorMessage = paymentResponse.ErrorMessage
                };
            }

            _logger.LogInformation ("Successfully authorized order payment");

            order.OrderStatus = OrderStatus.Comfirmed;
            await _orderRepository.UpdateOrder (order);

            var eventMessage = new OrderStatusConfirmedEvent {
                BookItems = order.Items.Select (_ => new BookItem (_.BookId, _.Quantity))
            };

            await PublishOrderConfimedEvent (eventMessage);

            return new AuthorizeOrderViewDto {
                OrderId = order.OrderId,
                OrderStatus = order.OrderStatus
            };
        } catch (System.Exception ex) {

            _logger.LogError (ex, ex.Message);
            return new AuthorizeOrderViewDto {
                ErrorMessage = "System Error"
            };
        }
    }

    private async Task PublishOrderConfimedEvent (OrderStatusConfirmedEvent eventMessage) {
        _logger.LogInformation ("Sending order status confirmed event to eventbus:{order}", JsonConvert.SerializeObject (eventMessage));

        await Policy.Handle<Exception> ().WaitAndRetryAsync (5,
                retryAttempt => TimeSpan.FromSeconds (Math.Pow (2, retryAttempt)),
                (exception, timespan, context) => {
                    _logger.LogError ("Error in publishing payload: {payload} to eventbus", JsonConvert.SerializeObject (eventMessage));

                    _logger.LogInformation ("Retrying to publish payload in {timespan}", timespan);
                })
            .ExecuteAsync (() => _publishEndpoint.Publish (eventMessage));

        _logger.LogInformation ("Sent order status confirmed event to eventbus");
    }

    private static FilterDefinition<Models.Order> BuildQuery (AuthorizeOrderCommand request) {
        var userOrdersFilter = Builders<Models.Order>.Filter.Eq (_ => _.UserId, request.UserId);
        var pendingOrderFilter = Builders<Models.Order>.Filter.Eq (_ => _.OrderStatus, OrderStatus.Pending);
        var userspendingFilter = Builders<Models.Order>.Filter.And (userOrdersFilter, pendingOrderFilter);
        return userspendingFilter;
    }
}