using AutoMapper;
using LibrarySystem.Api.Common;
using LibrarySystem.Api.Data;
using LibrarySystem.Api.Data.Repositories;
using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class BorrowerService
    : GenericService<Borrower, BorrowerReadDto,
        CreateBorrowerDto, UpdateBorrowerDto>,
      IBorrowerService
{
    private readonly IBorrowerRepository _borrowerRepository;

    public BorrowerService(
        IBorrowerRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<BorrowerService> logger)
        : base(repository, unitOfWork, mapper, logger)
    {
        _borrowerRepository = repository;
    }

    public async Task<Result<PagedResult<BorrowerReadDto>>> SearchAsync(
        string? searchTerm,
        DateTime? membershipDateFrom,
        DateTime? membershipDateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _borrowerRepository.query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(
                borrower => borrower.Name.Contains(term));
        }

        if (membershipDateFrom.HasValue)
        {
            query = query.Where(
                borrower => borrower.MembershipDate >=
                            membershipDateFrom.Value);
        }

        if (membershipDateTo.HasValue)
        {
            query = query.Where(
                borrower => borrower.MembershipDate <=
                            membershipDateTo.Value);
        }

        return await GetPagedAsync(
            query,
            pageNumber,
            pageSize,
            "Borrowers retrieved successfully.",
            cancellationToken);
    }

    public override async Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (await _borrowerRepository.HasLoansAsync(
                id, cancellationToken))
        {
            return Result<bool>.Conflict(
                "Cannot delete a borrower who has loan records.");
        }

        return await base.DeleteAsync(id, cancellationToken);
    }
}