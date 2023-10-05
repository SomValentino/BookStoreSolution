using AutoMapper;
using EventBus.Messages.Common;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Order.API.Application.Commands.CheckoutOrder;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Events;

public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent> {
    private readonly IMediator _mediator;
    
    private readonly ILogger<BasketCheckoutEvent> _logger;

    public BasketCheckoutConsumer (IMediator mediator,
        ILogger<BasketCheckoutEvent> logger) {
        _mediator = mediator;
        _logger = logger;
    }
    public async Task Consume (ConsumeContext<BasketCheckoutEvent> context) {
        var checkoutEvent = context.Message;
        if(checkoutEvent.Metadata.TryGetValue(EventBusConstants._correlationIdHeader,out string correlationId)){
            _logger.LogInformation("Consuming checkout event with correlationId: {id}",correlationId);
        }
        var command = new CheckoutOrderCommand {
            UserId = checkoutEvent.UserId,
            UserName = checkoutEvent.UserName,
            TotalPrice = checkoutEvent.TotalPrice,
            FirstName = checkoutEvent.FirstName,
            LastName = checkoutEvent.LastName,
            EmailAddress = checkoutEvent.EmailAddress,
            Items = checkoutEvent.Items
        };

        var result = await _mediator.Send (command);

        _logger.LogInformation ("BasketCheckoutEvent consumed successfully. Created Order Id : {newOrderId}", result);
    }
}