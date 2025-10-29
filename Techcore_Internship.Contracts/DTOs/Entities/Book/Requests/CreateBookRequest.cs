namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

public record CreateBookRequest(string Title, int Year, List<Guid> AuthorIds);
