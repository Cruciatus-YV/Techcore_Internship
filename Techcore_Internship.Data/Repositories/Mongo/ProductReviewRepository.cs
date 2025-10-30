using MongoDB.Driver;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo;

public class ProductReviewRepository : IProductReviewRepository
{
    private readonly IMongoCollection<ProductReviewEntity> _reviews;

    public ProductReviewRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Techcore_Internship_Mongo");
        _reviews = database.GetCollection<ProductReviewEntity>("reviews");
    }

    public async Task<ProductReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _reviews.Find(review => review.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(ProductReviewEntity review, CancellationToken cancellationToken)
    {
        review.Id = Guid.NewGuid();
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;
        await _reviews.InsertOneAsync(review, cancellationToken: cancellationToken);

        return review.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, ProductReviewEntity review, CancellationToken cancellationToken)
    {
        review.UpdatedAt = DateTime.UtcNow;
        var result = await _reviews.ReplaceOneAsync(r => r.Id == id, review, cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reviews.DeleteOneAsync(review => review.Id == id, cancellationToken);

        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
