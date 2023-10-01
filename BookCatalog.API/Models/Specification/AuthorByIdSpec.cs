using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class AuthorByIdSpec : Specification<Author>, ISingleResultSpecification<Author>{
    public AuthorByIdSpec(Guid id)
    {
        Query.Where(_ => _.AuthorId == id);
    }
}