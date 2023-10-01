using System.ComponentModel.DataAnnotations;

namespace BookCatalog.API.Dtos;

public record BookRecord {
    [Required]
    [MinLength(3)]
    public string Title { get; set; }
    [Required]
    [Range(1,long.MaxValue)]
    public long Price { get; set; }
    [Required]
    [Range(1,int.MaxValue)]
    public int Quantity { get; set; }
    public IEnumerable<Guid>? Authors { get; set; }
}