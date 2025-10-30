using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;

public class ProductDetailsResponse
{
    public BookResponse Book { get; set; }
    public List<ProductReviewResponse> Reviews { get; set; }

    public ProductDetailsResponse(BookResponse book, List<ProductReviewResponse> reviews)
    {
        Book = book;
        Reviews = reviews;
    }
}
