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

public class AuthorServiceTests {
    private readonly BookDbContext _context;
    private readonly EfRepository<Author> _repository;
    private readonly Mock<ILogger<AuthorService>> _loggerMock;
    private readonly Author _author;

    public AuthorServiceTests () {
        var options = new DbContextOptionsBuilder<BookDbContext> ()
            .UseInMemoryDatabase (Guid.NewGuid ().ToString ())
            .ConfigureWarnings (x => x.Ignore (InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new BookDbContext (options);
        _repository = new EfRepository<Author> (_context);
        _loggerMock = new Mock<ILogger<AuthorService>> ();

        _author = new Author {
            FirstName = "John",
            LastName = "test"
        };
    }

    [Fact]
    public async Task AuthorService_OnSave_SavesAuthorToDatabase () {
        var authorService = new AuthorService (_repository, _loggerMock.Object);

        await authorService.CreateAuthorAsync (_author);

        _context.Authors.Count ().Should ().BeGreaterThan (0);
        _context.Authors.Remove (_author);
    }

    [Fact]
    public async Task AuthorService_OnDelete_RemovesAuthorFromDatabase () {
        var authorService = new AuthorService (_repository, _loggerMock.Object);

        await authorService.CreateAuthorAsync (_author);

        await authorService.DeleteAuthorAsync (_author);

        _context.Authors.Count ().Should ().Be (0);
    }

    [Fact]
    public async Task AuthorService_OnUpdate_UpdatesAuthorToDatabase () {
        var authorService = new AuthorService (_repository, _loggerMock.Object);

        await authorService.CreateAuthorAsync (_author);

        _author.LastName = "UpdateName";

        await authorService.UpdateAuthorAsync (_author);

        _context.Authors.First ().LastName.Should ().Be ("UpdateName");
        _context.Authors.Remove (_author);
    }

    [Fact]
    public async Task AuthorService_GetAuthorById_ReturnsAuthor () {
        var authorService = new AuthorService (_repository, _loggerMock.Object);

        await authorService.CreateAuthorAsync (_author);

        var author = await authorService.GetAuthorByIdAsync(_author.AuthorId);

        author.AuthorId.Should ().Be (_author.AuthorId);
        _context.Authors.Remove (_author);
    }
}