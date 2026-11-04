using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Models;

namespace LibrarySystem.Api.Services;

public interface ILoanService
    : IGenericService<Loan, LoanReadDto,
        CreateLoanDto, UpdateLoanDto>
{
    Task<Result<PagedResult<LoanReadDto>>> SearchAsync(
        string? searchTerm,
        int? bookId,
        int? borrowerId,
        DateTime? fromDate,
        DateTime? toDate,
        bool? returned,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}