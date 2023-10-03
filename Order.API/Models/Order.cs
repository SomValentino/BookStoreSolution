using EventBus.Messages.Events;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.API.Models;

public class Order {
    [BsonId]
    public string OrderId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string UserName { get; set; }
    public long TotalPrice { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public List<ShoppingItem> Items { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}