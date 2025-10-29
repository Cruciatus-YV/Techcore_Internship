using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

public record CreateBookWithAuthorsRequest(string Title,
                                             int Year,
                                             List<CreateAuthorRequest> NewAuthors,
                                             List<Guid>? ExistingAuthorIds = null);
