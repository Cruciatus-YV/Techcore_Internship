namespace Techcore_Internship.AuthorsApi.Contracts.DTOs.Responses;

public record AuthorResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public List<BookReferenceResponse> Books { get; init; }

    public AuthorResponse() { }

    public AuthorResponse(Guid id, string firstName, string lastName, List<BookReferenceResponse> books)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Books = books;
    }
}
