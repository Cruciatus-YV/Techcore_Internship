using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;
using Techcore_Internship.Data.Repositories.Interfaces;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorResponseDto>> GetAll();
    Task<AuthorResponseDto?> Get(Guid id);
    Task<AuthorResponseDto> Create(CreateAuthorRequestDto author);
    Task<bool> Update(AuthorResponseDto request);
    Task<bool> UpdateName(Guid id, string firstName);
    Task<bool> Delete(Guid id);
    Task<bool> Exists(Guid id);
    Task<List<AuthorResponseDto>> GetAuthorsByFirstName(string firstName);
    Task<List<AuthorResponseDto>> GetAuthorsByLastName(string lastName);
}
