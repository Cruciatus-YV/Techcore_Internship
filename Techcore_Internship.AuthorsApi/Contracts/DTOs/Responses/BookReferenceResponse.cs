namespace Techcore_Internship.AuthorsApi.Contracts.DTOs.Responses;

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
}
