using BookCatalog.API.Dtos;
using BookCatalog.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.API.Controllers;

[ApiController]
//[Authorize]
[Route ("[controller]")]
public class AuthorsController : ControllerBase {
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController (IAuthorService authorService, ILogger<AuthorsController> logger) {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthors () {
        var authors = await _authorService.GetAuthorsAsync ();

        return Ok (authors);
    }

    [HttpGet ("{id}", Name = "GetAuthor")]
    public async Task<IActionResult> GetAuthor (Guid id) {
        var authors = await _authorService.GetAuthorByIdAsync (id);

        if (authors == null) return NotFound ();

        return Ok (authors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuthor ([FromBody] AuthorRecord authorRecord) {
        var author = new Author {
            FirstName = authorRecord.FirstName,
            LastName = authorRecord.LastName,
            CreatedAt = DateTime.UtcNow
        };

        author = await _authorService.CreateAuthorAsync (author);

        return CreatedAtAction ("GetAuthor", new { id = author.AuthorId }, author);
    }

    [HttpPut ("{id}")]
    public async Task<IActionResult> UpdateAuthor (Guid id, [FromBody] AuthorRecord authorRecord) {
        var author = await _authorService.GetAuthorByIdAsync (id);

        if (author == null) return NotFound ();

        author.FirstName = authorRecord.FirstName;
        author.LastName = authorRecord.LastName;
        author.UpdatedAt = DateTime.UtcNow;

        await _authorService.UpdateAuthorAsync (author);
        return NoContent ();
    }

    [HttpDelete ("{id}")]
    public async Task<IActionResult> DeleteAuthor (Guid id) {
        var author = await _authorService.GetAuthorByIdAsync (id);

        if (author == null) return NotFound ();

        await _authorService.DeleteAuthorAsync (author);

        return NoContent ();
    }

}