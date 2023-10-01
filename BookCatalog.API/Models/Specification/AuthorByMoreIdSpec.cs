using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class AuthorByMoreIdSpec : Specification<Author> {
    public AuthorByMoreIdSpec(IEnumerable<Guid> ids)
    {
        Query.Where(_ => ids.Contains(_.AuthorId));
    }
}