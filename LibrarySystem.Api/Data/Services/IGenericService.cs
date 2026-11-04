using LibrarySystem.Api.Common;

namespace LibrarySystem.Api.Services;

public interface IGenericService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TEntity : class, IEntity
    where TReadDto : class
{
    Task<Result<PagedResult<TReadDto>>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<Result<TReadDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<TReadDto>> CreateAsync(
        TCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<TReadDto>> UpdateAsync(
        int id,
        TUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}