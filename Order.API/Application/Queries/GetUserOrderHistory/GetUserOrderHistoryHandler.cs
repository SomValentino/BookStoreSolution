using AutoMapper;
using MediatR;
using MongoDB.Driver;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Queries.GetUserOrderHistory;

public class GetUserOrderHistoryHandler : IRequestHandler<GetUserOrderHistoryQuery, List<OrderViewDto>> {
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserOrderHistoryHandler> _logger;

    public GetUserOrderHistoryHandler (IOrderRepository orderRepository,
        IMapper mapper,
        ILogger<GetUserOrderHistoryHandler> logger) {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;

    }
    public async Task<List<OrderViewDto>> Handle (GetUserOrderHistoryQuery request, CancellationToken cancellationToken) 
    {
        var query = BuildOrderQuery (request);

        var orders = await _orderRepository.GetOrdersByQuery (query);

        var pagedOrders = orders.Skip ((request.Page - 1) * request.PageSize).Take (request.PageSize);

        var ordersViewDto = _mapper.Map<List<OrderViewDto>> (pagedOrders);

        return ordersViewDto;
    }

    private static FilterDefinition<Models.Order> BuildOrderQuery (GetUserOrderHistoryQuery request) {
        var userQuery = Builders<Models.Order>.Filter.Eq (_ => _.UserId, request.UserId);

        if (request.StartDate.HasValue) {
            var startDateQuery = Builders<Models.Order>.Filter.Gte (_ => _.CreatedAt, request.StartDate);

            userQuery = Builders<Models.Order>.Filter.And (userQuery, startDateQuery);
        }

        if (request.EndDate.HasValue) {
            var endDateQuery = Builders<Models.Order>.Filter.Lte (_ => _.CreatedAt, request.EndDate);

            userQuery = Builders<Models.Order>.Filter.And (userQuery, endDateQuery);
        }

        return userQuery;
    }
}