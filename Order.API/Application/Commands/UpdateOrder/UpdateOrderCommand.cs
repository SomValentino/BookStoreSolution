using EventBus.Messages.Events;
using MediatR;
using Order.API.Models;

namespace Order.API.Application.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest {
    public string OrderId { get; set; }

    public long TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }

    public List<ShoppingItem> Items { get; set; }
}