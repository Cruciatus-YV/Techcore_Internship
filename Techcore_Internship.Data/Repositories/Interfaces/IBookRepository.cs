using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Interfaces;

public interface IBookRepository : IGenericRepository<BookEntity, Guid>
{
    Task<List<BookEntity>> GetAllWithAuthorsAsync(CancellationToken cancellationToken);
    Task<List<BookEntity>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);
    Task<BookEntity?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken);
    Task<List<BookEntity>> GetByYearAsync(int year, CancellationToken cancellationToken);
}
