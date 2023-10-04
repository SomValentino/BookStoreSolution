using BookCatalog.API.Data;
using BookCatalog.API.Data.Repository;
using BookCatalog.API.Models;
using BookCatalog.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookCatalog.API.Tests;

public class BookCatalogServiceTests {
    private readonly BookDbContext _context;
    private readonly EfRepository<Book> _repository;
    private readonly Mock<ILogger<BookCatalogService>> _loggerMock;
    private readonly Book _book;

    public BookCatalogServiceTests () {
        var options = new DbContextOptionsBuilder<BookDbContext> ()
            .UseInMemoryDatabase (Guid.NewGuid ().ToString ())
            .ConfigureWarnings (x => x.Ignore (InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new BookDbContext (options);
        _repository = new EfRepository<Book> (_context);
        _loggerMock = new Mock<ILogger<BookCatalogService>> ();

        _book = new Book {
            Title = "BookTest",
            Price = 100,
            Quantity = 56,
            CreatedAt = DateTime.UtcNow,
            Authors = new List<Author> {
            new Author {
            FirstName = "TestAuthor",
            LastName = "TestAuthor"
            }
            }
        };
    }

    [Fact]
    public async Task BookCatalogService_OnCreate_SavesBook () {
        var bookCatalogService = new BookCatalogService (_repository, _loggerMock.Object);

        await bookCatalogService.CreateBookAsync (_book);

        _context.Books.Count ().Should ().BeGreaterThan (0);
        _context.Books.Remove (_book);
    }

    [Fact]
    public async Task BookCatalogService_OnDelete_RemovesBook () {
        var bookCatalogService = new BookCatalogService (_repository, _loggerMock.Object);

        await bookCatalogService.CreateBookAsync (_book);

        await bookCatalogService.DeleteBookAsync (_book);

        _context.Books.Count ().Should ().Be (0);
    }

    [Fact]
    public async Task BookCatalogService_OnUpdate_UpdateBook () {
        var bookCatalogService = new BookCatalogService (_repository, _loggerMock.Object);

        await bookCatalogService.CreateBookAsync (_book);

        _book.Title = "BookUpdated";

        await bookCatalogService.UpdateBookAsync (_book);

        _context.Books.First ().Title.Should ().Be ("BookUpdated");
        _context.Books.Remove (_book);
    }

    [Fact]
    public async Task BookCatalogService_GetByBookByTitle_ReturnsBookThatHasTitle () {
        var bookCatalogService = new BookCatalogService (_repository, _loggerMock.Object);

        await bookCatalogService.CreateBookAsync (_book);

        var searchBooks = await bookCatalogService.GetBooksAsync ("test");

        searchBooks.Count ().Should ().BeGreaterThan (0);
        _context.Books.Remove (_book);
    }

    [Fact]
    public async Task BookCatalogService_GetByBookByTitleThatDoesNotExist_ReturnsBookList () {
        var bookCatalogService = new BookCatalogService (_repository, _loggerMock.Object);

        await bookCatalogService.CreateBookAsync (_book);

        var searchBooks = await bookCatalogService.GetBooksAsync ("testdsdsd");

        searchBooks.Count ().Should ().Be (0);
        _context.Books.Remove (_book);
    }
}