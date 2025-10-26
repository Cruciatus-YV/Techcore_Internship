using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Data.Repositories.Interfaces;

public interface IGenericRepository<TEntity, TId> where TEntity : IHaveId<TId> where TId : struct
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TId> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<List<TId>> InsertListAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> Exist(TId id, CancellationToken cancellationToken = default);
}
