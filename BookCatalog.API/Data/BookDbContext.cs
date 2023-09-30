using System.Reflection;
using BookCatalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookCatalog.API.Data;

public class BookDbContext : DbContext {
    public BookDbContext (DbContextOptions<BookDbContext> options) : base (options) {

    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }


    protected override void OnModelCreating (ModelBuilder modelBuilder) {
        base.OnModelCreating (modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly (Assembly.GetExecutingAssembly ());
    }

}