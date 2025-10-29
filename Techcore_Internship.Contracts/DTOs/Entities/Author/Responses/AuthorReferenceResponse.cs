using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

public record AuthorReferenceResponse(Guid Id, string FirstName, string LastName)
{
    public AuthorReferenceResponse(AuthorEntity author) : this (author.Id, author.FirstName, author.LastName)
    {
        
    }
};