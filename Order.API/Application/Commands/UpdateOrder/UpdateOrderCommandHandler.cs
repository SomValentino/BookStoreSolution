using AutoMapper;
using MediatR;
using Order.API.Application.Exceptions;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand> {
    private IOrderRepository _orderRepository;
    private ILogger<UpdateOrderCommand> _logger;

    public UpdateOrderCommandHandler (IOrderRepository orderRepository, ILogger<UpdateOrderCommand> logger) {
        _orderRepository = orderRepository;
        _logger = logger;
    }
    public async Task<Unit> Handle (UpdateOrderCommand request, CancellationToken cancellationToken) 
    {
        var order = await _orderRepository.GetOrderById (request.OrderId);

        if (order == null) {
            throw new NotFoundException (nameof (order), request.OrderId);
        }

        order.TotalPrice = request.TotalPrice;
        order.Items =request.Items;
        order.OrderStatus = request.OrderStatus;

        await _orderRepository.UpdateOrder (order);

        _logger.LogInformation ($"Order {order.OrderId} is successfully updated.");

        return Unit.Value;
    }
}