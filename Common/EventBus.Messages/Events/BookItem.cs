namespace EventBus.Messages.Events;

public class BookItem {
    public string BookId { get; set; }
    public int Quantity { get; set; }

    public BookItem(string bookId, int quantity)
    {
        BookId = bookId;
        Quantity = quantity;
    }
}