using EventBus.Messages.Events;
using Order.API.Models;

namespace Order.API.Application.Queries;

public record OrderViewDto {
    public string OrderId { get; set; }
    public long TotalPrice { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public List<ShoppingItem> Items { get; set; }
    public DateTime CreatedAt { get; set; }
}