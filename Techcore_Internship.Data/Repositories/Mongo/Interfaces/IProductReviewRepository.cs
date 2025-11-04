using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo.Interfaces;

public interface IProductReviewRepository
{
    Task<ProductReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ProductReviewEntity>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<List<ProductReviewEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<ProductReviewEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<Guid> CreateAsync(ProductReviewEntity review, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, ProductReviewEntity review, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}