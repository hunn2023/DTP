using DTP.Shared.Domain;
using DTP.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DTP.Shared.Infrastructure.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
     where TEntity : EntityBase
    {
        protected readonly DbContext Context;

        protected readonly DbSet<TEntity> DbSet;

        public RepositoryBase(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id && !x.IsDeleted,
                    cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            entity.Delete();
        }

        public virtual async Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AnyAsync(
                    x => x.Id == id && !x.IsDeleted,
                    cancellationToken);
        }

        public virtual async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
