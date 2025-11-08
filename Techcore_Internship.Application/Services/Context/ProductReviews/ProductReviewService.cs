using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Context.ProductReviews;

public class ProductReviewService : IProductReviewService
{
    private readonly IProductReviewRepository _productReviewRepository;

    public ProductReviewService(IProductReviewRepository productReviewRepository)
    {
        _productReviewRepository = productReviewRepository;
    }

    public async Task<ProductReviewResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await _productReviewRepository.GetByIdAsync(id, cancellationToken);
        return review != null ? new ProductReviewResponse(review) : null;
    }

    public async Task<List<ProductReviewResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var reviews = await _productReviewRepository.GetByProductIdAsync(productId, cancellationToken);
        return reviews.Select(review => new ProductReviewResponse(review)).ToList();
    }

    public async Task<List<ProductReviewResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var reviews = await _productReviewRepository.GetByUserIdAsync(userId, cancellationToken);
        return reviews.Select(review => new ProductReviewResponse(review)).ToList();
    }

    public async Task<List<ProductReviewResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var reviews = await _productReviewRepository.GetAllAsync(cancellationToken);
        return reviews.Select(review => new ProductReviewResponse(review)).ToList();
    }

    public async Task<Guid> CreateAsync(CreateProductReviewRequest request, CancellationToken cancellationToken)
    {
        var review = new ProductReviewEntity
        {
            ProductId = request.ProductId,
            UserId = request.UserId,
            UserName = request.UserName,
            Rating = request.Rating,
            Title = request.Title,
            Comment = request.Comment,
            IsVerified = request.IsVerified,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _productReviewRepository.CreateAsync(review, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductReviewRequest request, CancellationToken cancellationToken)
    {
        var existingReview = await _productReviewRepository.GetByIdAsync(id, cancellationToken);

        if (existingReview == null)
            return false;

        var updatedReview = new ProductReviewEntity
        {
            Id = id,
            ProductId = request.ProductId,
            UserId = request.UserId,
            UserName = request.UserName,
            Rating = request.Rating,
            Title = request.Title,
            Comment = request.Comment,
            IsVerified = request.IsVerified,
            CreatedAt = existingReview.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        return await _productReviewRepository.UpdateAsync(id, updatedReview, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _productReviewRepository.DeleteAsync(id, cancellationToken);
    }
}