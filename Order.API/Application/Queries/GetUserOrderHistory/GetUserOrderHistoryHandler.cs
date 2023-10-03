using AutoMapper;
using EventBus.Messages.Events;
using MediatR;
using MongoDB.Driver;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Application.Queries.GetUserOrderHistory;

public class GetUserOrderHistoryHandler : IRequestHandler<GetUserOrderHistoryQuery, List<OrderViewDto>> {
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetUserOrderHistoryHandler> _logger;

    public GetUserOrderHistoryHandler (IOrderRepository orderRepository,
        ILogger<GetUserOrderHistoryHandler> logger) {
        _orderRepository = orderRepository;
        _logger = logger;

    }
    public async Task<List<OrderViewDto>> Handle (GetUserOrderHistoryQuery request, CancellationToken cancellationToken) {
        var query = BuildOrderQuery (request);

        var orders = await _orderRepository.GetOrdersByQuery (query, _ => _.CreatedAt, false,
                                     request.Page, request.PageSize);

        var ordersViewDto = orders.Select (_ => new OrderViewDto {
            OrderId = _.OrderId,
                TotalPrice = _.TotalPrice,
                FirstName = _.FirstName,
                LastName = _.LastName,
                OrderStatus = _.OrderStatus,
                EmailAddress = _.EmailAddress,
                Items = _.Items.Select (_ => new ShoppingItem {
                    Quantity = _.Quantity,
                        Price = _.Price,
                        BookId = _.BookId,
                        BookTitle = _.BookTitle,
                }).ToList (),
                CreatedAt = _.CreatedAt
        });

        return ordersViewDto.ToList ();
    }

    private static FilterDefinition<Models.Order> BuildOrderQuery (GetUserOrderHistoryQuery request) {
        var userQuery = Builders<Models.Order>.Filter.Eq (_ => _.UserId, request.UserId);

        if (request.OrderStatus.HasValue) {
            var orderStatusQuery = Builders<Models.Order>.Filter.Eq (_ => _.OrderStatus, request.OrderStatus);

            userQuery = Builders<Models.Order>.Filter.And (userQuery, orderStatusQuery);
        }

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