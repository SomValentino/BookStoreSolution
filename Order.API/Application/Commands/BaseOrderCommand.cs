using EventBus.Messages.Events;

namespace Order.API.Application.Commands;

public class BaseOrderCommand {
    public string UserId { get; set; }
    public string UserName { get; set; }
    public long TotalPrice { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public List<ShoppingItem> Items { get; set; }
}