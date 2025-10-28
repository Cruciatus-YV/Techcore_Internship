using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

public record UpdateBookRequest(string Title, int Year, List<CreateAuthorRequest>? NewAuthors = null, List<Guid>? ExistingAuthorIds = null);
