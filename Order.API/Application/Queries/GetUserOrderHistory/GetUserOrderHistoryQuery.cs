

using MediatR;
using Order.API.Models;

namespace Order.API.Application.Queries.GetUserOrderHistory;

public class GetUserOrderHistoryQuery : IRequest<List<OrderViewDto>> 
{
    public string UserId { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}