using Microsoft.EntityFrameworkCore;
using Techcore_Internship.AuthorsApi.Data.Interfaces;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.EF;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.AuthorsApi.Data.Repositories
{
    public class AuthorRepository(ApplicationDbContext dbContext) : GenericRepository<AuthorEntity, Guid>(dbContext), IAuthorRepository
    {
        public async Task<AuthorEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Authors
            .AsNoTracking()
            .Include(a => a.Books.Where(b => !b.IsDeleted))
            .FirstOrDefaultAsync(a => !a.IsDeleted && a.Id == id, cancellationToken);
        }

        public async Task<List<AuthorEntity>> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Authors
                .Where(a => requestedIds.Contains(a.Id) && !a.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AuthorEntity>> GetAllAsync(bool includeBooks = false, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Authors
                .AsNoTracking()
                .Where(a => !a.IsDeleted);

            if (includeBooks)
            {
                query = query.Include(a => a.Books.Where(b => !b.IsDeleted));
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
