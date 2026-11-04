using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Books;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;

namespace LibrarySystem.Api.Data.Services
{
    public interface IBookService:IGenericService<Book, BookReadDto,CreateBookDto, UpdateBookDto>
    {
        Task<Result<PagedResult<BookReadDto>>> SearchAsync(
        string? searchTerm,
        int? authorId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    }
}
