using AutoMapper;
using LibrarySystem.Api.Common;
using LibrarySystem.Api.Data;
using LibrarySystem.Api.Data.Repositories;
using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class LoanService
    : GenericService<Loan, LoanReadDto,
        CreateLoanDto, UpdateLoanDto>,
      ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowerRepository _borrowerRepository;

    public LoanService(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IBorrowerRepository borrowerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LoanService> logger)
        : base(loanRepository, unitOfWork, mapper, logger)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _borrowerRepository = borrowerRepository;
    }

    public async Task<Result<PagedResult<LoanReadDto>>> SearchAsync(
        string? searchTerm,
        int? bookId,
        int? borrowerId,
        DateTime? fromDate,
        DateTime? toDate,
        bool? returned,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _loanRepository.query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(loan =>
                loan.Book.Title.Contains(term) ||
                loan.Borrower.Name.Contains(term));
        }

        if (bookId.HasValue)
        {
            query = query.Where(
                loan => loan.BookId == bookId.Value);
        }

        if (borrowerId.HasValue)
        {
            query = query.Where(
                loan => loan.BorrowerId == borrowerId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(
                loan => loan.LoanDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(
                loan => loan.LoanDate <= toDate.Value);
        }

        if (returned.HasValue)
        {
            query = returned.Value
                ? query.Where(loan => loan.ReturnDate.HasValue)
                : query.Where(loan => !loan.ReturnDate.HasValue);
        }

        return await GetPagedAsync(
            query,
            pageNumber,
            pageSize,
            "Loans retrieved successfully.",
            cancellationToken);
    }

    public override async Task<Result<LoanReadDto>> CreateAsync(
        CreateLoanDto dto,
        CancellationToken cancellationToken = default)
    {
        if (IsInvalidDateRange(dto.LoanDate, dto.ReturnDate))
        {
            return Result<LoanReadDto>.Failure(
                "ReturnDate cannot be earlier than LoanDate.");
        }

        var book = await _bookRepository.GetByIdAsync(
            dto.BookId, cancellationToken);

        if (book is null)
        {
            return Result<LoanReadDto>.Failure(
                $"Book with ID {dto.BookId} does not exist.");
        }

        var borrower = await _borrowerRepository.GetByIdAsync(
            dto.BorrowerId, cancellationToken);

        if (borrower is null)
        {
            return Result<LoanReadDto>.Failure(
                $"Borrower with ID {dto.BorrowerId} does not exist.");
        }

        return await base.CreateAsync(dto, cancellationToken);
    }

    public override async Task<Result<LoanReadDto>> UpdateAsync(
        int id,
        UpdateLoanDto dto,
        CancellationToken cancellationToken = default)
    {
        var existingLoan = await _loanRepository.GetByIdAsync(
            id, cancellationToken);

        if (existingLoan is null)
        {
            return Result<LoanReadDto>.NotFound(
                $"Loan with ID {id} was not found.");
        }

        if (IsInvalidDateRange(dto.LoanDate, dto.ReturnDate))
        {
            return Result<LoanReadDto>.Failure(
                "ReturnDate cannot be earlier than LoanDate.");
        }

        var book = await _bookRepository.GetByIdAsync(
            dto.BookId, cancellationToken);

        if (book is null)
        {
            return Result<LoanReadDto>.Failure(
                $"Book with ID {dto.BookId} does not exist.");
        }

        var borrower = await _borrowerRepository.GetByIdAsync(
            dto.BorrowerId, cancellationToken);

        if (borrower is null)
        {
            return Result<LoanReadDto>.Failure(
                $"Borrower with ID {dto.BorrowerId} does not exist.");
        }

        return await base.UpdateAsync(id, dto, cancellationToken);
    }

    private static bool IsInvalidDateRange(
        DateTime loanDate,
        DateTime? returnDate)
    {
        return returnDate.HasValue &&
               returnDate.Value < loanDate;
    }
}