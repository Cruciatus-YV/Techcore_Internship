using Microsoft.EntityFrameworkCore;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.EF;

public class BookRepository(ApplicationDbContext dbContext) : GenericRepository<BookEntity, Guid>(dbContext), IBookRepository
{
    public async Task<List<BookEntity>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        return await _asNoTracking
            .Include(b => b.Authors)
            .ToListAsync(cancellationToken);
    }

    public async Task<BookEntity?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _asNoTracking
            .Include(b => b.Authors.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);
    }

    public async Task<List<BookEntity>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _asNoTracking
            .Include(b => b.Authors.Where(a => !a.IsDeleted))
            .Where(b => !b.IsDeleted && b.Authors.Any(a => a.Id == authorId))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BookEntity>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        return await _asNoTracking
            .Include(b => b.Authors.Where(a => !a.IsDeleted))
            .Where(b => !b.IsDeleted && b.Year == year)
            .ToListAsync(cancellationToken);
    }
}
