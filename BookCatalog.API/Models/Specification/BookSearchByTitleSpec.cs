using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookSearchByTitleSpec : Specification<Book> {
    public BookSearchByTitleSpec(string title)
    {
        Query.Where(_ => _.Title.Contains(title))
             .Include(_ => _.Authors);
    }
}