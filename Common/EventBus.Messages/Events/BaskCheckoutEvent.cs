namespace EventBus.Messages.Events {
    public class BasketCheckoutEvent : IntegrationBaseEvent {
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }
}