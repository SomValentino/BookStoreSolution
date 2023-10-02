using EventBus.Messages.Events;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.API.Models;

public class Order {
    [BsonId]
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public List<ShoppingItem> Items { get; set; }
}