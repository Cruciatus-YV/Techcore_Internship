using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Data.Repositories.EF.Interfaces;

public interface IGenericRepository<TEntity, TId> where TEntity : IHaveId<TId> where TId : struct
{
    Task<TEntity?> GetEntityByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllEntitiesAsync(CancellationToken cancellationToken = default);
    Task<List<TId>> InsertListEntityAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<bool> UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> IsEntityExists(TId id, CancellationToken cancellationToken = default);
    Task InsertEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
}
