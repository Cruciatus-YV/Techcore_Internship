namespace Techcore_Internship.Contracts.DTOs.Requests;

public record CreateBookRequestDto(string Title, int Year, bool IsDeleted, Guid AuthorId);
