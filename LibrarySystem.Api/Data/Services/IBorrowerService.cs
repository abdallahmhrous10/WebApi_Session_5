using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Models;

namespace LibrarySystem.Api.Services;

public interface IBorrowerService
    : IGenericService<Borrower, BorrowerReadDto,
        CreateBorrowerDto, UpdateBorrowerDto>
{
    Task<Result<PagedResult<BorrowerReadDto>>> SearchAsync(
        string? searchTerm,
        DateTime? membershipDateFrom,
        DateTime? membershipDateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}