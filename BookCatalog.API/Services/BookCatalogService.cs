using BookCatalog.API.Data;
using BookCatalog.API.Models;
using BookCatalog.API.Services.interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Services;

public class BookCatalogService : IBookCatalogService {
    private readonly BookDbContext _bookDbContext;
    private readonly ILogger<BookCatalogService> _logger;

    public BookCatalogService (BookDbContext bookDbContext, ILogger<BookCatalogService> logger) {
        _bookDbContext = bookDbContext;
        _logger = logger;
    }
    public async Task<Book> CreateBookAsync (Book book) {
        using var transaction = _bookDbContext.Database.BeginTransaction ();
        try {
            await _bookDbContext.AddAsync (book);
            await _bookDbContext.SaveChangesAsync ();
            transaction.Commit ();
            return book;
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }

    public async Task DeleteBookAsync (Book book) {
        using var transaction = _bookDbContext.Database.BeginTransaction ();
        try {
            _bookDbContext.Entry (book).State = EntityState.Deleted;
            _bookDbContext.Books.Remove (book);
            await _bookDbContext.SaveChangesAsync ();
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }

    public async Task<Book> GetBookByIdAsync (Guid id) {
        return await _bookDbContext.Books.FirstOrDefaultAsync (_ => _.BookId == id);
    }

    public async Task<IEnumerable<Book>> GetBooksAsync (string? title = null) {
        if (string.IsNullOrEmpty (title)) {
            return await _bookDbContext.Books.ToListAsync ();
        }

        return await _bookDbContext.Books.Where (_ => _.Title.Contains (title)).ToListAsync ();
    }

    public async Task UpdateBookAsync (Book book) {
        using var transaction = _bookDbContext.Database.BeginTransaction ();
        try {
            _bookDbContext.Entry (book).State = EntityState.Modified;
            _bookDbContext.Books.Remove (book);
            await _bookDbContext.SaveChangesAsync ();
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }
}