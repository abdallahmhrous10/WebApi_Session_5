using LibrarySystem.Api.Common;

namespace LibrarySystem.Api.Data.Repositories
{
    public interface IGenericRepository <T> where T : class, IEntity
    {
        IQueryable<T> query();

        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);
        void Delete(T entity);
    }
}
