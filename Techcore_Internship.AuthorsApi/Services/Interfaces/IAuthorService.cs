using Techcore_Internship.AuthorsApi.Contracts.DTOs.Requests;
using Techcore_Internship.AuthorsApi.Contracts.DTOs.Responses;

namespace Techcore_Internship.AuthorsApi.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorReferenceResponse> CreateAsync(CreateAuthorRequest request);
        Task<AuthorReferenceResponse> GetByIdAsync(Guid id);
        Task<List<AuthorReferenceResponse>> GetAllAsync();
        Task<AuthorReferenceResponse> UpdateAsync(Guid id, UpdateAuthorInfoRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
