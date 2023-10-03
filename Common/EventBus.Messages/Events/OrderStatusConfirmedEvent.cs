namespace EventBus.Messages.Events;

public class OrderStatusConfirmedEvent : IntegrationBaseEvent {
    public IEnumerable<BookItem> BookItems { get; set; }
}