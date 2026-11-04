using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Data.Repositories;

public class BorrowerRepository
    : GenericRepository<Borrower>, IBorrowerRepository
{
    public BorrowerRepository(LibraryDbContext _context)
        : base(_context)
    {
    }

    public Task<bool> HasLoansAsync(
        int borrowerId,
        CancellationToken cancellationToken = default)
    {
        return _context.Loans.AnyAsync(
            loan => loan.BorrowerId == borrowerId,
            cancellationToken);
    }
}