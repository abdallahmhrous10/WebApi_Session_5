using LibrarySystem.Api.Models;

namespace LibrarySystem.Api.Data.Repositories
{
    public interface IAuthorRepository: IGenericRepository<Author>
    {
        Task<bool> HasBooksAsync(
      int authorId,
      CancellationToken cancellationToken = default);
    }
}
