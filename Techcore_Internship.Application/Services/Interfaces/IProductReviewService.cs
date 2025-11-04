using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IProductReviewService
{
    Task<ProductReviewResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ProductReviewResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<List<ProductReviewResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<ProductReviewResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Guid> CreateAsync(CreateProductReviewRequest review, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateProductReviewRequest review, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}