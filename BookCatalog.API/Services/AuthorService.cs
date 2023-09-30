using BookCatalog.API.Data;
using BookCatalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Services;

public class AuthorService : IAuthorService {
    private readonly BookDbContext _bookDbContext;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService (BookDbContext bookDbContext, ILogger<AuthorService> logger) {
        _bookDbContext = bookDbContext;
        _logger = logger;
    }
    public async Task<Author> CreateAuthorAsync (Author author) {
        using var transaction = _bookDbContext.Database.BeginTransaction ();
        try {
            await _bookDbContext.AddAsync (author);
            await _bookDbContext.SaveChangesAsync ();
            transaction.Commit ();
            return author;
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }

    public async Task DeleteAuthorAsync (Author author) {
        using var transaction = _bookDbContext.Database.BeginTransaction ();
        try {
            _bookDbContext.Entry (author).State = EntityState.Deleted;
            _bookDbContext.Authors.Remove (author);
            await _bookDbContext.SaveChangesAsync ();
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }

    public async Task<Author> GetAuthorByIdAsync (Guid id) {
        return await _bookDbContext.Authors.FirstOrDefaultAsync (_ => _.AuthorId == id);
    }

    public async Task<IEnumerable<Author>> GetAuthorsAsync () {
        return await _bookDbContext.Authors.ToListAsync ();
    }

    public async Task<IEnumerable<Author>> GetAuthorsAsync (IEnumerable<Guid> ids) {
        return await _bookDbContext.Authors.Where(_ => ids.Contains (_.AuthorId)).ToListAsync();
    }

    public async Task UpdateAuthorAsync (Author author) {
        using var transaction = _bookDbContext.Database.BeginTransaction ();
        try {
            _bookDbContext.Entry (author).State = EntityState.Modified;
            _bookDbContext.Authors.Remove (author);
            await _bookDbContext.SaveChangesAsync ();
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }
}