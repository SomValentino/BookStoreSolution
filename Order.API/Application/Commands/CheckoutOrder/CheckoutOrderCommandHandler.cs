using AutoMapper;
using MediatR;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Guid> {
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler (IOrderRepository orderRepository,
        IMapper mapper,
        ILogger<CheckoutOrderCommandHandler> logger) {
        _repository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Guid> Handle (CheckoutOrderCommand request, CancellationToken cancellationToken) {
        var order = _mapper.Map<Order.API.Models.Order> (request);

        await _repository.CreateOrder (order);

        _logger.LogInformation ($"Order {order.OrderId} is successfully created.");

        return order.OrderId;
    }
}