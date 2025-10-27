using Microsoft.EntityFrameworkCore;
using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories
{
    public class AuthorRepository(ApplicationDbContext dbContext) : GenericRepository<AuthorEntity, Guid>(dbContext), IAuthorRepository
    {
        public async Task<List<AuthorEntity>> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Authors
                .AsNoTracking()
                .Where(a => requestedIds.Contains(a.Id) && !a.IsDeleted)
                .ToListAsync(cancellationToken);
        }
    }
}
