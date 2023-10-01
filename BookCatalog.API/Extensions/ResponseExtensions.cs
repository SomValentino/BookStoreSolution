using BookCatalog.API.Dtos;
using BookCatalog.API.Models;

namespace BookCatalog.API.Extensions;

public static class ResponseExtensions{
    public static BookViewRecord ToBookViewDto(this Book book){
        return new BookViewRecord{
            BookId = book.BookId,
            Title = book.Title,
            Price = book.Price,
            Quantity = book.Quantity,
            Authors = book.Authors.Select(_ => new AuthorViewRecord { 
                AuthorId = _.AuthorId,
                FirstName = _.FirstName,
                LastName = _.LastName
            })
        };
    }

    public static AuthorViewRecord ToAuthorViewDto(this Author author){
        return new AuthorViewRecord {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName
        };
    }
}