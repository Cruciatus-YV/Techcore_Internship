using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

public record AuthorResponse(Guid Id, string FirstName, string LastName, List<BookReferenceResponse> Books)
{
    public AuthorResponse(AuthorEntity author) 
        : this (author.Id,
                author.FirstName, 
                author.LastName, 
                author.Books?.Select(book => new BookReferenceResponse(book)).ToList() ?? new())
    {
        
    }
};
