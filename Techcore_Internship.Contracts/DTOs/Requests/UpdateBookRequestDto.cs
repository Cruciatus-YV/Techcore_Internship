namespace Techcore_Internship.Contracts.DTOs.Requests;

public record UpdateBookRequestDto(string Title, int Year, List<CreateAuthorRequestDto>? NewAuthors = null, List<Guid>? ExistingAuthorIds = null);
