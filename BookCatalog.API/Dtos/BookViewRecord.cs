namespace BookCatalog.API.Dtos;

public record BookViewRecord {
    public Guid BookId { get; set; }
    public string Title { get; set; }
    public long Price { get; set; }
    public int Quantity { get; set; }
    public bool HasStock { get; set; }
    public IEnumerable<AuthorViewRecord> Authors { get; set; }
}