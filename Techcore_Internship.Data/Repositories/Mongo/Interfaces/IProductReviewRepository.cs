using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo.Interfaces;

public interface IProductReviewRepository
{
    Task<Guid> CreateAsync(ProductReviewEntity review, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, ProductReviewEntity review, CancellationToken cancellationToken);
}
