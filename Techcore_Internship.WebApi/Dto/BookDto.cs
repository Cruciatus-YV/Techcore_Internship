namespace Techcore_Internship.WebApi.Dto;

public record BookDto(Guid Id, string Title, string Author, int Year) : CreateBookDto(Title, Author, Year);
