using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookByIdsSpec : Specification<Book>, ISingleResultSpecification<Book> {
    public BookByIdsSpec (IEnumerable<Guid> bookIds){
        Query.Where (_ => bookIds.Contains (_.BookId));
    }
}