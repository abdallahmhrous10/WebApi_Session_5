using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Data.Repositories
{
    public class AuthorRepository: GenericRepository<Author>, IAuthorRepository
    {

        public AuthorRepository(LibraryDbContext _context):base(_context)
        {
            
        }

        public Task<bool> HasBooksAsync(  int authorId, CancellationToken cancellationToken = default)
       {
            return _context.Books.AnyAsync(
         book => book.AuthorId == authorId,
         cancellationToken);
        }
    }
}
