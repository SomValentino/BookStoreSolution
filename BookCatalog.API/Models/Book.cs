using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCatalog.API.Models;

public class Book {
    [Key]
    public Guid BookId { get; set; }
    [Required]
    public string Title { get; set; }   
    public int Quantity { get; set; }
    public long Price { get; set; }
    [NotMapped]
    public bool HasStock => Quantity > 0;
    public ICollection<Author> Authors { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}