using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Author;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;

namespace LibrarySystem.Api.Data.Services
{
    public interface IAuthorService:IGenericService<Author,AuthorReadDto,CreateAuthorDto,UpdateAuthorDto>
    {
        public Task<Result<PagedResult<AuthorReadDto>>> SearchAsync(
              string? searchTerm,
              DateTime? birthDateFrom,
              DateTime? birthDateTo,
              int PageNumber = 1,
              int PageSize = 10,
              CancellationToken cancellationToken = default);
            
    }
}
