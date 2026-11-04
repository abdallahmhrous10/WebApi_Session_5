using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Author;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;
using LibrarySystem.Api.Data.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
namespace LibrarySystem.Api.Data.Services
{
    public class AuthorService
     : GenericService<Author, AuthorReadDto,
         CreateAuthorDto, UpdateAuthorDto>, IAuthorService
    {

        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository repository, IUnitOfWork unitOfWork , IMapper mapper, ILogger<AuthorService> logger)
            : base(repository, unitOfWork, mapper, logger)
        {
            _authorRepository = repository;
        }
        public async Task<Result<PagedResult<AuthorReadDto>>> SearchAsync(string? searchTerm ,DateTime? birthDateFrom, DateTime? birthDateTo, int PageNumber = 1, int PageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _authorRepository.query().AsNoTracking();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                query = query.Where(
                    author => author.Name.Contains(term));
            }

            if (birthDateFrom.HasValue)
            {
                query = query.Where(
                    author => author.BirthDate >= birthDateFrom.Value);
            }

            if (birthDateTo.HasValue)
            {
                query = query.Where(
                    author => author.BirthDate <= birthDateTo.Value);
            }

            return await GetPagedAsync(
                query,
                PageNumber,
                PageSize,
                "Authors retrieved successfully.",
                cancellationToken);
        }

        public override async Task<Result<bool>> DeleteAsync(
              int id,
              CancellationToken cancellationToken = default)
         {
            if (await _authorRepository.HasBooksAsync(
                    id, cancellationToken))
            {
                return Result<bool>.Conflict(
                    "Cannot delete an author who has books.");
            }

            return await base.DeleteAsync(id, cancellationToken);
        }
    }
}
