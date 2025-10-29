using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

public record UpdateBookAuthorsRequest(List<CreateAuthorRequest>? NewAuthors = null, List<Guid>? ExistingAuthorIds = null);
