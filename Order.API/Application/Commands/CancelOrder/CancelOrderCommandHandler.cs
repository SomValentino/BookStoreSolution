using Ardalis.GuardClauses;
using MediatR;
using Order.API.Data.Repository.Interfaces;
using Order.API.Models;

namespace Order.API.Application.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand> {
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler (IOrderRepository orderRepository, ILogger<CancelOrderCommandHandler> logger) {
        _orderRepository = orderRepository;
        _logger = logger; 
    }
    public async Task<Unit> Handle (CancelOrderCommand request, CancellationToken cancellationToken) {
        var order = await _orderRepository.GetOrderById(request.OrderId);

        Guard.Against.Null(order,nameof(order));

        order.OrderStatus = OrderStatus.Cancelled;

        await _orderRepository.UpdateOrder(order);

        return Unit.Value;
    }
}