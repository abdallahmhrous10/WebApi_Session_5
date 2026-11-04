using LibrarySystem.Api.Models;

namespace LibrarySystem.Api.Data.Repositories;

public interface IBorrowerRepository
    : IGenericRepository<Borrower>
{
    Task<bool> HasLoansAsync(
        int borrowerId,
        CancellationToken cancellationToken = default);
}