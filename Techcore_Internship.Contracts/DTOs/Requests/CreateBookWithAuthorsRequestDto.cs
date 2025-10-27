namespace Techcore_Internship.Contracts.DTOs.Requests;

public record CreateBookWithAuthorsRequestDto(string Title,
                                             int Year,
                                             List<CreateAuthorRequestDto> NewAuthors,
                                             List<Guid>? ExistingAuthorIds = null);
