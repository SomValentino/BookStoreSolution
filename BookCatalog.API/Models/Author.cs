using System.ComponentModel.DataAnnotations;

namespace BookCatalog.API.Models;

public class Author {
    [Key]
    public Guid AuthorId { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public ICollection<Book> Books { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}