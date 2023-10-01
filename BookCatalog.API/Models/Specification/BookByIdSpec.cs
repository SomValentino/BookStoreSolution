using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookByIdSpec: Specification<Book>, ISingleResultSpecification<Book> 
{
    public BookByIdSpec(Guid id)
    {
        Query.Where(_ => _.BookId == id)
             .Include(_ => _.Authors);
    }
}