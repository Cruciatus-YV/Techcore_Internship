using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Responses;

public record AuthorReferenceResponseDto(Guid Id, string FirstName, string LastName)
{
    public AuthorReferenceResponseDto(AuthorEntity author) : this (author.Id, author.FirstName, author.LastName)
    {
        
    }
};