using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibrarySystem.Api.Common;
using LibrarySystem.Api.Data;
using LibrarySystem.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class GenericService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    : IGenericService<TEntity, TReadDto, TCreateDto, TUpdateDto>
    where TEntity : class, IEntity
    where TReadDto : class
{
    protected readonly IGenericRepository<TEntity> Repository;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper Mapper;
    protected readonly ILogger Logger;

    public GenericService(
        IGenericRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
        Mapper = mapper;
        Logger = logger;
    }

    public virtual async Task<Result<PagedResult<TReadDto>>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            Repository.query().AsNoTracking(),
            pageNumber,
            pageSize,
            "Records retrieved successfully.",
            cancellationToken);
    }

    public virtual async Task<Result<TReadDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var dto = await Repository.query()
            .AsNoTracking()
            .Where(entity => entity.Id == id)
            .ProjectTo<TReadDto>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            Logger.LogWarning(
                "Entity {EntityType} with ID {EntityId} was not found.",
                typeof(TEntity).Name,
                id);

            return Result<TReadDto>.NotFound(
                $"{typeof(TEntity).Name} with ID {id} was not found.");
        }

        return Result<TReadDto>.Success(
            dto,
            "Record retrieved successfully.");
    }

    public virtual async Task<Result<TReadDto>> CreateAsync(
        TCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = Mapper.Map<TEntity>(dto);

        await Repository.AddAsync(entity, cancellationToken);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var readDto = await GetReadDtoAsync(
            entity.Id,
            cancellationToken);

        Logger.LogInformation(
            "Entity {EntityType} with ID {EntityId} was created.",
            typeof(TEntity).Name,
            entity.Id);

        return Result<TReadDto>.Success(
            readDto,
            "Record created successfully.",
            201);
    }

    public virtual async Task<Result<TReadDto>> UpdateAsync(
        int id,
        TUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(
            id,
            cancellationToken);

        if (entity is null)
        {
            return Result<TReadDto>.NotFound(
                $"{typeof(TEntity).Name} with ID {id} was not found.");
        }

        Mapper.Map(dto, entity);

        Repository.Update(entity);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var readDto = await GetReadDtoAsync(
            id,
            cancellationToken);

        Logger.LogInformation(
            "Entity {EntityType} with ID {EntityId} was updated.",
            typeof(TEntity).Name,
            id);

        return Result<TReadDto>.Success(
            readDto,
            "Record updated successfully.");
    }

    public virtual async Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(
            id,
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.NotFound(
                $"{typeof(TEntity).Name} with ID {id} was not found.");
        }

        Repository.Delete(entity);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        Logger.LogInformation(
            "Entity {EntityType} with ID {EntityId} was deleted.",
            typeof(TEntity).Name,
            id);

        return Result<bool>.Success(
            true,
            "Record deleted successfully.");
    }

    protected async Task<Result<PagedResult<TReadDto>>> GetPagedAsync(
        IQueryable<TEntity> query,
        int pageNumber,
        int pageSize,
        string message,
        CancellationToken cancellationToken = default)
    {
        var projectedQuery = query
            .OrderBy(entity => entity.Id)
            .ProjectTo<TReadDto>(Mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<TReadDto>.CreateAsync(
            projectedQuery,
            pageNumber,
            pageSize,
            cancellationToken);

        Logger.LogInformation(
            "Retrieved {EntityType} page {PageNumber} with page size {PageSize}.",
            typeof(TEntity).Name,
            pagedResult.PageNumber,
            pagedResult.PageSize);

        return Result<PagedResult<TReadDto>>.Success(
            pagedResult,
            message);
    }

    private async Task<TReadDto> GetReadDtoAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await Repository.query()
            .AsNoTracking()
            .Where(entity => entity.Id == id)
            .ProjectTo<TReadDto>(Mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}