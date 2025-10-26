using Techcore_Internship.Contracts.DTOs.Requests;

namespace Techcore_Internship.Contracts.DTOs.Responses;

public record BookResponseDto(Guid Id, string Title, int Year, bool IsDeleted, Guid AuthorId) : CreateBookRequestDto(Title, Year, IsDeleted, AuthorId);