using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookSearchByTitleSpec : Specification<Book> {
    public BookSearchByTitleSpec(string title)
    {
        Query.Where(_ => _.Title.ToLower().Contains(title.ToLower()))
             .Include(_ => _.Authors);
    }
}