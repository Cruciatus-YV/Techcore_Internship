using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

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

    public AuthorResponse(AuthorEntity author)
        : this(author.Id, author.FirstName, author.LastName,
               author.Books?.Select(book => new BookReferenceResponse(book)).ToList() ?? new())
    {
    }
}