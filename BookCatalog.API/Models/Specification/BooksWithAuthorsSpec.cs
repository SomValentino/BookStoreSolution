using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookWithAuthorsSpec : Specification<Book> {
    public BookWithAuthorsSpec () {
        Query.Where (_ => true).Include (_ => _.Authors);
    }
}