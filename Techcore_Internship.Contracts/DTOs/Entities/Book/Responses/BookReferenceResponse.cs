using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

public record BookReferenceResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public int Year { get; init; }

    public BookReferenceResponse() { }

    public BookReferenceResponse(Guid id, string title, int year)
    {
        Id = id;
        Title = title;
        Year = year;
    }

    public BookReferenceResponse(BookEntity book)
        : this(book.Id, book.Title, book.Year)
    {
    }
}