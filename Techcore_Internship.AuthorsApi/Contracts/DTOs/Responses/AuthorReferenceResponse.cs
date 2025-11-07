using Techcore_Internship.AuthorsApi.Domain;

namespace Techcore_Internship.AuthorsApi.Contracts.DTOs.Responses;

public record AuthorReferenceResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }

    public AuthorReferenceResponse() { }

    public AuthorReferenceResponse(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public AuthorReferenceResponse(AuthorEntity author)
        : this(author.Id, author.FirstName, author.LastName)
    {
    }
}
