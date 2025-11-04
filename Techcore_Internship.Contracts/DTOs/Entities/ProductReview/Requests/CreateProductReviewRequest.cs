namespace Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Requests;

public record CreateProductReviewRequest
{
    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; }

    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsVerified { get; set; }
}
