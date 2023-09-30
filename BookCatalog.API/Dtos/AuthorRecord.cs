using System.ComponentModel.DataAnnotations;

namespace BookCatalog.API.Dtos;

public record AuthorRecord {
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}