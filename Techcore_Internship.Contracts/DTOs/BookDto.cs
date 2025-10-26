namespace Techcore_Internship.Contracts.DTOs;

public record BookDto(Guid Id, string Title, string Author, int Year) : CreateBookDto(Title, Author, Year);
