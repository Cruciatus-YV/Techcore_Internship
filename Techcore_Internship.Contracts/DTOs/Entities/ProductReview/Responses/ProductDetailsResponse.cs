using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

namespace Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;

public class ProductDetailsResponse
{
    public BookResponse Book { get; set; }
    public List<ProductReviewResponse> Reviews { get; set; }
    public string AvgBookRating { get; set; }

    public ProductDetailsResponse(BookResponse book, List<ProductReviewResponse> reviews, string avgBookRating)
    {
        Book = book;
        Reviews = reviews;
        AvgBookRating = avgBookRating;
    }
}
