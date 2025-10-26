using Techcore_Internship.Contracts.DTOs.Requests;

namespace Techcore_Internship.Contracts.DTOs.Responses;

public record AuthorResponseDto(Guid Id, string FirstName, string LastName, bool IsDeleted) : CreateAuthorRequestDto(FirstName, LastName, IsDeleted);
