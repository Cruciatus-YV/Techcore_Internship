using MongoDB.Driver;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo;

public class BookViewAnalyticsRepository : IBookViewAnalyticsRepository
{
    private readonly IMongoCollection<BookViewAnalyticsEntity> _analytics;

    public BookViewAnalyticsRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Techcore_Internship_Mongo");
        _analytics = database.GetCollection<BookViewAnalyticsEntity>("book_view_analytics");
    }

    public async Task CreateAsync(BookViewAnalyticsEntity analytics, CancellationToken cancellationToken)
    {
        analytics.Id = Guid.NewGuid();
        analytics.ProcessedAt = DateTime.UtcNow;
        await _analytics.InsertOneAsync(analytics, cancellationToken: cancellationToken);
    }

    public async Task<List<BookViewAnalyticsEntity>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken)
    {
        return await _analytics
            .Find(a => a.BookId == bookId)
            .SortByDescending(a => a.ViewDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetViewCountByBookIdAsync(Guid bookId, CancellationToken cancellationToken)
    {
        return await _analytics
            .CountDocumentsAsync(a => a.BookId == bookId, cancellationToken: cancellationToken);
    }
}
