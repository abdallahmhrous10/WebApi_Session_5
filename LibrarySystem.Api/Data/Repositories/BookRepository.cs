using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Data.Repositories;

public class BookRepository
    : GenericRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext _context)
        : base(_context)
    {
    }

    public async Task<bool> IsbnExistsAsync(
        string isbn,
        int? excludedBookId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Books
            .Where(book => book.ISBN == isbn);

        if (excludedBookId.HasValue)
        {
            query = query.Where(
                book => book.Id != excludedBookId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public Task<bool> HasLoansAsync(
        int bookId,
        CancellationToken cancellationToken = default)
    {
        return _context.Loans.AnyAsync(
            loan => loan.BookId == bookId,
            cancellationToken);
    }
}