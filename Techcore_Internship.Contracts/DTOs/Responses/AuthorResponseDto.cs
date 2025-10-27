using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Responses;

public record AuthorResponseDto(Guid Id, string FirstName, string LastName, List<BookReferenceResponseDto> Books)
{
    public AuthorResponseDto(AuthorEntity author) 
        : this (author.Id,
                author.FirstName, 
                author.LastName, 
                author.Books?.Select(book => new BookReferenceResponseDto(book)).ToList() ?? new())
    {
        
    }
};
