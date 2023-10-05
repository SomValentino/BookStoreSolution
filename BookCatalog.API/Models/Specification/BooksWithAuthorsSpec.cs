using Ardalis.Specification;

namespace BookCatalog.API.Models.Specification;

public class BookWithAuthorsSpec : Specification<Book> {
    public BookWithAuthorsSpec (int page = 1, int pageSize = 10){
        Query.Where (_ => true).Skip((page - 1)*pageSize).Take(pageSize).Include (_ => _.Authors);
    }
}