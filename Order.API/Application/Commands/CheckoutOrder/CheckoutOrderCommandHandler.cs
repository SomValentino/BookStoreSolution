using AutoMapper;
using MediatR;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, string> {
    private readonly IOrderRepository _repository;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler (IOrderRepository orderRepository,
        ILogger<CheckoutOrderCommandHandler> logger) {
        _repository = orderRepository;
        _logger = logger;
    }
    public async Task<string> Handle (CheckoutOrderCommand request, CancellationToken cancellationToken) {
        var order = new Models.Order{
            UserId = request.UserId,
            UserName = request.UserName,
            TotalPrice = request.TotalPrice,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailAddress = request.EmailAddress,
            Items = request.Items
        };

        await _repository.CreateOrder (order);

        _logger.LogInformation ($"Order {order.OrderId} is successfully created.");

        return order.OrderId;
    }
}