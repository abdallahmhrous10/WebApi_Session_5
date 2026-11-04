using LibrarySystem.Api.Models;

namespace LibrarySystem.Api.Data.Repositories;

public class LoanRepository
    : GenericRepository<Loan>, ILoanRepository
{
    public LoanRepository(LibraryDbContext context)
        : base(context)
    {
    }
}