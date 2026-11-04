using LibrarySystem.Api.Common;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class,IEntity
    {
        protected readonly LibraryDbContext _context;
        protected readonly DbSet<T> dbSet;

        public GenericRepository(LibraryDbContext context)
        {
            _context = context;
            dbSet = context.Set<T>();
        }

        public IQueryable<T> query()
        {
            return dbSet;
        }

        public async Task<T?> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default)
        {
            return await dbSet.FindAsync(
                new object?[] { id },
                cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
           await dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public void Delete (T entity)
        {
           dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
         dbSet.Update(entity);
        }
    }
}
