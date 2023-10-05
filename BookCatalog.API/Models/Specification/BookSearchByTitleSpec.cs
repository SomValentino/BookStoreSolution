using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookSearchByTitleSpec : Specification<Book> {
    public BookSearchByTitleSpec (string title,int page = 1, int pageSize = 10){
        Query.Where (_ => _.Title.ToLower ().Contains (title.ToLower ()))
            .Skip ((page - 1) * pageSize).Take (pageSize)
            .Include (_ => _.Authors);
    }
}