using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.AuthorsApi.Data.Interfaces;

public interface IAuthorRepository : IGenericRepository<AuthorEntity, Guid>
{
    Task<List<AuthorEntity>> GetAllAsync(bool includeBooks = false, CancellationToken cancellationToken = default);
    Task<AuthorEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AuthorEntity>> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default);
}
