namespace EventBus.Messages.Common {
    public static class EventBusConstants {
        public const string BasketCheckoutQueue = "basketcheckout-queue";
        public const string OrderStatusConfirmedQueue = "orderstatusconfirmed-queue";
        public const string _correlationIdHeader = "X-Correlation-Id";
    }
}