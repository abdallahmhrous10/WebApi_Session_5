using LibrarySystem.Api.Models;

namespace LibrarySystem.Api.Data.Repositories;

public interface IBookRepository
    : IGenericRepository<Book>
{
    Task<bool> IsbnExistsAsync(
        string isbn,
        int? excludedBookId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasLoansAsync(
        int bookId,
        CancellationToken cancellationToken = default);
}