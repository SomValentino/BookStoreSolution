using BookCatalog.API.Models;

namespace BookCatalog.API.Services.interfaces;

public interface IBookCatalogService {
    Task<Book> CreateBookAsync (Book book);

    Task UpdateBookAsync (Book book);

    Task UpdateBookAsync (IEnumerable<Book> book);

    Task<IEnumerable<Book>> GetBooksAsync (string? title = null,int page = 1, int pageSize = 10);

    Task<Book> GetBookByIdAsync (Guid id);

    Task<IEnumerable<Book>> GetBookByIdsAsync (IEnumerable<Guid> ids);

    Task DeleteBookAsync (Book book);
}