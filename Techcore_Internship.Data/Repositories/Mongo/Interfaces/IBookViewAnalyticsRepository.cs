using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo.Interfaces;

public interface IBookViewAnalyticsRepository
{
    Task CreateAsync(BookViewAnalyticsEntity analytics, CancellationToken cancellationToken);
    Task<List<BookViewAnalyticsEntity>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken);
    Task<long> GetViewCountByBookIdAsync(Guid bookId, CancellationToken cancellationToken);
}
