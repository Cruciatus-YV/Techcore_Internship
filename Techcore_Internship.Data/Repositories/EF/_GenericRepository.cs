using Microsoft.EntityFrameworkCore;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Data.Repositories.EF;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
where TEntity : class, IHaveId<TId>
where TId : struct
{
    public readonly ApplicationDbContext _dbContext;
    public readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    public async Task InsertEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TId>> InsertListEntityAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entities.Select(x => x.Id).ToList();
    }

    public async Task<List<TEntity>> GetAllEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetEntityByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<bool> UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Update(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> IsEntityExists(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.AsNoTracking().CountAsync(x => x.Id.Equals(id), cancellationToken);

        return entity > 0;
    }
}