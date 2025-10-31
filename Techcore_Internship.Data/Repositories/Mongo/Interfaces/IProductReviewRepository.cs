using System.Linq.Expressions;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Mongo.Interfaces;

public interface IProductReviewRepository
{
    Task<Guid> CreateAsync(ProductReviewEntity review, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductReviewEntity?> GetByPredicateAsync(Expression<Func<ProductReviewEntity, bool>> predicate, CancellationToken cancellationToken);
    Task<List<ProductReviewEntity>> GetListByPredicateAsync(Expression<Func<ProductReviewEntity, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, ProductReviewEntity review, CancellationToken cancellationToken);
}
