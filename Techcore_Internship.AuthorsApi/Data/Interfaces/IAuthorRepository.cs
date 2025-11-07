using Techcore_Internship.AuthorsApi.Domain;

namespace Techcore_Internship.AuthorsApi.Data.Interfaces;

public interface IAuthorRepository
{
    Task<Guid> CreateAsync(AuthorEntity author);
    Task<List<AuthorEntity>> GetAllAsync();
    Task<AuthorEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(AuthorEntity author);
    Task<bool> DeleteAsync(Guid id);
}
