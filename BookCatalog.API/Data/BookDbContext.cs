using System.Reflection;
using BookCatalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Data;

public class BookDbContext : DbContext {
    public BookDbContext (DbContextOptions<BookDbContext> options) : base (options) {

    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    public override async Task<int> SaveChangesAsync (CancellationToken cancellationToken = default) {
        var rows = 0;
        using var transaction = this.Database.BeginTransaction ();
        try {
            rows = await base.SaveChangesAsync (cancellationToken);
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            throw;
        }
        return rows;
    }

    protected override void OnModelCreating (ModelBuilder modelBuilder) {
        base.OnModelCreating (modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}