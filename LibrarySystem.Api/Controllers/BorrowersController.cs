using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[Authorize(Roles = "Admin,Librarian")]
[ApiController]
[Route("api/[controller]")]
public class BorrowersController : ControllerBase
{
    private readonly IBorrowerService _service;

    public BorrowersController(IBorrowerService service)
    {
        _service = service;
    }

    // GET: api/borrowers
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] DateTime? membershipDateFrom,
        [FromQuery] DateTime? membershipDateTo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.SearchAsync(
            searchTerm,
            membershipDateFrom,
            membershipDateTo,
            pageNumber,
            pageSize,
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // GET: api/borrowers/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // POST: api/borrowers
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBorrowerDto dto,
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

    // PUT: api/borrowers/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBorrowerDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            id, dto, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/borrowers/1
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