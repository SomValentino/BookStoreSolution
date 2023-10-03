using AutoMapper;
using MediatR;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Queries.GetOrder;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderViewDto> 
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler (IOrderRepository orderRepository,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _repository = orderRepository;
        _logger = logger;
    }
    public async Task<OrderViewDto> Handle (GetOrderByIdQuery request, CancellationToken cancellationToken) 
    {
        var order = await _repository.GetOrderById(request.OrderId);

        var orderViewDto = new OrderViewDto{
            OrderId = order.OrderId,
            TotalPrice = order.TotalPrice,
            FirstName = order.FirstName,
            LastName = order.LastName,
            EmailAddress = order.EmailAddress,
            OrderStatus = order.OrderStatus,
            Items = order.Items,
            CreatedAt = order.CreatedAt
        };

        return orderViewDto;
    }
}