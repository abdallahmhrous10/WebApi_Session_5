using LibrarySystem.Api.Common;
using LibrarySystem.Api.Data.Services;
using LibrarySystem.Api.Dtos.Author;
using LibrarySystem.Api.Dtos.Author;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _service;

    public AuthorsController(IAuthorService service)
    {
        _service = service;
    }

    // GET: api/authors
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] DateTime? birthDateFrom,
        [FromQuery] DateTime? birthDateTo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.SearchAsync(
            searchTerm,
            birthDateFrom,
            birthDateTo,
            pageNumber,
            pageSize,
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // GET: api/authors/1
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

    // POST: api/authors
    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAuthorDto dto,
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

    // PUT: api/authors/1
    [Authorize(Roles = "Admin,Librarian")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateAuthorDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            id, dto, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/authors/1
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