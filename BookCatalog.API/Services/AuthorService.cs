using BookCatalog.API.Data;
using BookCatalog.API.Data.Repository;
using BookCatalog.API.Models;
using BookCatalog.API.Models.Specification;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Services;

public class AuthorService : IAuthorService {
    private readonly IRepository<Author> _authorRepository;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService (IRepository<Author> authorRepository, ILogger<AuthorService> logger) {
        _authorRepository = authorRepository;
        _logger = logger;
    }
    public async Task<Author> CreateAuthorAsync (Author author) {
        using var transaction = _authorRepository.DbContext.Database.BeginTransaction ();
        try {
            await _authorRepository.AddAsync (author);
            transaction.Commit ();
            return author;
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }

    public async Task DeleteAuthorAsync (Author author) {
        using var transaction = _authorRepository.DbContext.Database.BeginTransaction ();
        try {
            await _authorRepository.DeleteAsync (author);
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }

    public async Task<Author> GetAuthorByIdAsync (Guid id) {
        return await _authorRepository.GetBySpecAsync(new AuthorByIdSpec (id));
    }

    public async Task<IEnumerable<Author>> GetAuthorsAsync () {
        return await _authorRepository.ListAsync ();
    }

    public async Task<IEnumerable<Author>> GetAuthorsAsync (IEnumerable<Guid> ids) {
        return await _authorRepository.ListAsync (new AuthorByMoreIdSpec (ids));
    }

    public async Task UpdateAuthorAsync (Author author) {
        using var transaction = _authorRepository.DbContext.Database.BeginTransaction ();
        try {
            await _authorRepository.UpdateAsync (author);
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            throw;
        }
    }
}