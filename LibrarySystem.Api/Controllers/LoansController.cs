using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[Authorize(Roles = "Admin,Librarian")]
[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _service;

    public LoansController(ILoanService service)
    {
        _service = service;
    }

    // GET: api/loans
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] int? bookId,
        [FromQuery] int? borrowerId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] bool? returned,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.SearchAsync(
            searchTerm,
            bookId,
            borrowerId,
            fromDate,
            toDate,
            returned,
            pageNumber,
            pageSize,
            cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // GET: api/loans/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // POST: api/loans
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLoanDto dto,
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

    // PUT: api/loans/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateLoanDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            id, dto, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/loans/1
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