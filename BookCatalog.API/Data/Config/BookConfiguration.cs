using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookCatalog.API.Models;

namespace BookCatalog.API.Data.Configuration;

public class BookCatalogConfiguration : IEntityTypeConfiguration<Book>{
    public void Configure(EntityTypeBuilder<Book> BookBuilder){
        BookBuilder.HasIndex(_ => _.Title).IsUnique();
        BookBuilder.HasMany(_ => _.Authors)
                   .WithMany(_ => _.Books);
                
    }
}