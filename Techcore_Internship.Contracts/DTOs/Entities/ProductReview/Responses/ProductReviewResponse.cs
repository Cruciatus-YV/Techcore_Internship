using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;

public record ProductReviewResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsVerified { get; init; }

    public ProductReviewResponse(Guid id, Guid userId, string userName, int rating, string title, string comment, DateTime createdAt, bool isVerified)
    {
        Id = id;
        UserId = userId;
        UserName = userName;
        Rating = rating;
        Title = title;
        Comment = comment;
        CreatedAt = createdAt;
        IsVerified = isVerified;
    }
    public ProductReviewResponse(ProductReviewEntity reviewEntity) : this(reviewEntity.Id,
                                                                          reviewEntity.UserId,
                                                                          reviewEntity.UserName,
                                                                          reviewEntity.Rating,
                                                                          reviewEntity.Title,
                                                                          reviewEntity.Comment,
                                                                          reviewEntity.CreatedAt,
                                                                          reviewEntity.IsVerified)
    { 
    }
}
