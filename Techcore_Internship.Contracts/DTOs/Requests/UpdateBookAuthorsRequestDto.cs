namespace Techcore_Internship.Contracts.DTOs.Requests;

public record UpdateBookAuthorsRequestDto(List<CreateAuthorRequestDto>? NewAuthors = null, List<Guid>? ExistingAuthorIds = null);
