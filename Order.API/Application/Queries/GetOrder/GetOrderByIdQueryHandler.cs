using AutoMapper;
using MediatR;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Queries.GetOrder;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderViewDto> 
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler (IOrderRepository orderRepository,
        IMapper mapper,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _repository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<OrderViewDto> Handle (GetOrderByIdQuery request, CancellationToken cancellationToken) 
    {
        var order = await _repository.GetOrderById(request.OrderId);

        var orderViewDto = _mapper.Map<OrderViewDto>(order);

        return orderViewDto;
    }
}