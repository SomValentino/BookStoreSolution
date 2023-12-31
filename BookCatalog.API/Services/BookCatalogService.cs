using BookCatalog.API.Data;
using BookCatalog.API.Data.Repository;
using BookCatalog.API.Models;
using BookCatalog.API.Models.Specification;
using BookCatalog.API.Services.interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Services;

public class BookCatalogService : IBookCatalogService {
    private readonly IRepository<Book> _bookRepository;
    private readonly ILogger<BookCatalogService> _logger;

    public BookCatalogService (IRepository<Book> bookRepository, ILogger<BookCatalogService> logger) {
        _bookRepository = bookRepository;
        _logger = logger;
    }
    public async Task<Book> CreateBookAsync (Book book) {
        using var transaction = _bookRepository.DbContext.Database.BeginTransaction ();
        try {
            await _bookRepository.AddAsync (book);
            transaction.Commit ();
            return book;
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
            return default !;
        }
    }

    public async Task DeleteBookAsync (Book book) {
        using var transaction = _bookRepository.DbContext.Database.BeginTransaction ();
        try {
            await _bookRepository.DeleteAsync (book);
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
        }
    }

    public async Task<Book> GetBookByIdAsync (Guid id) {
        var book = await _bookRepository.GetBySpecAsync (new BookByIdSpec (id));

        return book!;
    }

    public async Task<IEnumerable<Book>> GetBookByIdsAsync (IEnumerable<Guid> ids) {
        var books = await _bookRepository.ListAsync (new BookByIdsSpec (ids));

        return books;
    }

    public async Task<IEnumerable<Book>> GetBooksAsync (string? title = null, int page = 1, int pageSize = 10) {
        if (!string.IsNullOrEmpty (title)) {
            return await _bookRepository.ListAsync (new BookSearchByTitleSpec (title!,page,pageSize));
        }

        return await _bookRepository.ListAsync (new BookWithAuthorsSpec (page,pageSize));
    }

    public async Task UpdateBookAsync (Book book) {
        using var transaction = _bookRepository.DbContext.Database.BeginTransaction ();
        try {
            await _bookRepository.UpdateAsync (book);
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
        }
    }

    public async Task UpdateBookAsync (IEnumerable<Book> book) {
        using var transaction = _bookRepository.DbContext.Database.BeginTransaction ();
        try {
            await _bookRepository.UpdateRangeAsync (book);
            transaction.Commit ();
        } catch (System.Exception ex) {
            transaction.Rollback ();
            _logger.LogError (ex, ex.Message);
        }
    }
}