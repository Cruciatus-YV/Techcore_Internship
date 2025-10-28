using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.EF.Interfaces;

public interface IBookRepository : IGenericRepository<BookEntity, Guid>
{
    Task<List<BookEntity>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
    Task<List<BookEntity>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<BookEntity?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<BookEntity>> GetByYearAsync(int year, CancellationToken cancellationToken = default);
}
