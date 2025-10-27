using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Interfaces;

public interface IAuthorRepository : IGenericRepository<AuthorEntity, Guid>
{
    Task<List<AuthorEntity>> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default);
}
