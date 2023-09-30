using BookCatalog.API.Models;

public interface IAuthorService {
    Task<Author> GetAuthorByIdAsync (Guid id);

    Task<IEnumerable<Author>> GetAuthorsAsync ();

    Task<IEnumerable<Author>> GetAuthorsAsync (IEnumerable<Guid> ids);

    Task<Author> CreateAuthorAsync(Author author);

    Task UpdateAuthorAsync (Author author);

    Task DeleteAuthorAsync (Author author);
}