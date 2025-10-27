using Microsoft.EntityFrameworkCore;
using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Data.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
where TEntity : class, IHaveId<TId>
where TId : struct
{
    private protected readonly ApplicationDbContext _dbContext;
    private protected readonly DbSet<TEntity> _dbSet;
    private protected readonly IQueryable<TEntity> _asNoTracking;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
        _asNoTracking = _dbSet.AsNoTracking();
    }

    public async Task<TId> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<List<TId>> InsertListAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entities.Select(x => x.Id).ToList();
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _asNoTracking.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _asNoTracking.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Update(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Exist(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _asNoTracking.CountAsync(x => x.Id.Equals(id), cancellationToken);

        return entity > 0;
    }
}