using LibrarySystem.Api.Data.Services;
using LibrarySystem.Api.Dtos.Books;
using LibrarySystem.Api.Dtos.Books;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;

    public BooksController(IBookService service)
    {
        _service = service;
    }

    // GET: api/books
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] int? authorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.SearchAsync(
            searchTerm,
            authorId,
            pageNumber,
            pageSize,
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // GET: api/books/1
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // POST: api/books
    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            dto, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                result);
        }

        return StatusCode(result.StatusCode, result);
    }

    // PUT: api/books/1
    [Authorize(Roles = "Admin,Librarian")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBookDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            id, dto, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/books/1
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(
            id, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}