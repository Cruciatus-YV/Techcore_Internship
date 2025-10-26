namespace Techcore_Internship.Contracts.DTOs.Requests;

public record CreateBookWithAuthorRequestDto(string Title,
                                             int Year,
                                             string AuthorFirstName,
                                             string AuthorLastName);
