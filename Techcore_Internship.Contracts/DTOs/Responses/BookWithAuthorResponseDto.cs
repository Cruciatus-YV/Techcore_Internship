namespace Techcore_Internship.Contracts.DTOs.Responses;

public record BookWithAuthorResponseDto(Guid Id,
                                        string Title,
                                        int Year,
                                        bool IsDeleted,
                                        Guid AuthorId,
                                        string AuthorFirstName,
                                        string AuthorLastName);
