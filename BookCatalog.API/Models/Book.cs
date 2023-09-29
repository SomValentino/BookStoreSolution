namespace BookCatalog.API.Models;

public class Book {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Quantity { get; set; }
    public long Price { get; set; }
    public ICollection<Author> Authors { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}