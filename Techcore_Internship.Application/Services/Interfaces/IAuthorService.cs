using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IAuthorService
{
    Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default, bool includeBooks = false);
    Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AuthorResponse>?> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default);
    Task<bool> IsExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateAuthorInfoRequest request, CancellationToken cancellationToken = default);
}
