namespace EventBus.Messages.Events {
    public class BasketCheckoutEvent : IntegrationBaseEvent {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public long TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public List<ShoppingItem> Items { get; set; }
    }
}