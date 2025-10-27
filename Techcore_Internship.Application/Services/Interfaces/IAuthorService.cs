using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;
using Techcore_Internship.Data.Repositories.Interfaces;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorResponseDto>?> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken);
}
