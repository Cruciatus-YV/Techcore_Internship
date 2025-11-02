using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

public record BookResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public int Year { get; init; }
    public List<AuthorReferenceResponse> Authors { get; init; }

    public BookResponse() { }

    public BookResponse(Guid id, string title, int year, List<AuthorReferenceResponse> authors)
    {
        Id = id;
        Title = title;
        Year = year;
        Authors = authors;
    }

    public BookResponse(BookEntity book)
        : this(book.Id, book.Title, book.Year,
               book.Authors?.Select(a => new AuthorReferenceResponse(a)).ToList() ?? new())
    {
    }
}