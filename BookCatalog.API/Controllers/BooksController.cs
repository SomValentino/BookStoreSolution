using BookCatalog.API.Data;
using BookCatalog.API.Dtos;
using BookCatalog.API.Extensions;
using BookCatalog.API.Models;
using BookCatalog.API.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Controllers;

[ApiController]
//[Authorize]
[Route ("[controller]")]
public class BooksController : ControllerBase {
    private readonly IBookCatalogService _bookCatalogService;
    private readonly IAuthorService _authorService;
    private readonly ILogger<BooksController> _logger;

    public BooksController (IBookCatalogService bookCatalogService,
        IAuthorService authorService,
        ILogger<BooksController> logger) {
        _bookCatalogService = bookCatalogService;
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet ("all")]
    public async Task<IActionResult> GetBooks (string? title = null) {

        var books = await _bookCatalogService.GetBooksAsync (title);

        return Ok (books.Select(_ => _.ToBookViewDto()));
    }

    [HttpGet ("{id}")]
    public async Task<IActionResult> GetBook (Guid id) {
        var book = await _bookCatalogService.GetBookByIdAsync (id);

        if (book == null) return NotFound ();

        return Ok (book.ToBookViewDto());
    }

    //[Authorize (Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateBook ([FromBody] BookRecord bookRecord) {

        if (!bookRecord.Authors.Any ()) return BadRequest ();

        var authors = await _authorService.GetAuthorsAsync (bookRecord.Authors.Distinct ());

        if (!authors.Any ()) return BadRequest ();

        var book = new Book {
            Title = bookRecord.Title,
            Price = bookRecord.Price,
            Quantity = bookRecord.Quantity,
            Authors = authors.ToList (),
            CreatedAt = DateTime.UtcNow
        };

        book = await _bookCatalogService.CreateBookAsync (book);

        return Ok (book.ToBookViewDto());
    }

    //[Authorize (Roles = "Administrator")]
    [HttpPut ("id")]
    public async Task<IActionResult> UpdateBook (Guid id, [FromBody] BookRecord bookRecord) {
        var book = await _bookCatalogService.GetBookByIdAsync (id);

        if (book == null) return NotFound ();

        if (bookRecord.Authors.Any ()) {
            book.Authors.Clear ();

            var authors = await _authorService.GetAuthorsAsync (bookRecord.Authors.Distinct ());

            if (!authors.Any ()) return BadRequest ();

            book.Authors = authors.ToList ();
        }

        book.Title = bookRecord.Title;
        book.Quantity = bookRecord.Quantity;
        book.Price = bookRecord.Price;
        book.UpdatedAt = DateTime.UtcNow;

        await _bookCatalogService.UpdateBookAsync (book);

        return NoContent ();
    }

    //[Authorize (Roles = "Administrator")]
    [HttpDelete]
    public async Task<IActionResult> DeleteBook (Guid id) {
        var book = await _bookCatalogService.GetBookByIdAsync (id);

        if (book == null) return NotFound ();

        await _bookCatalogService.DeleteBookAsync (book);

        return NoContent ();
    }
}