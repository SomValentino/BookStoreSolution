namespace BookCatalog.API.Dtos;

public record AuthorViewRecord {
    public Guid AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}