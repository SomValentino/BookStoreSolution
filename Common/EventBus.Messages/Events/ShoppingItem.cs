namespace EventBus.Messages.Events {
    public class ShoppingItem {
        public int Quantity { get; set; }
        public long Price { get; set; }
        public string BookId { get; set; }
        public string BookTitle { get; set; }
    }
}