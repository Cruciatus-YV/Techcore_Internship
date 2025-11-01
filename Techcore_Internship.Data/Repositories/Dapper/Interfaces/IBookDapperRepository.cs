using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

namespace Techcore_Internship.Data.Repositories.Dapper.Interfaces;

public interface IBookDapperRepository : IBaseDapperRepository
{
    Task<List<BookResponse>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
    Task<BookResponse?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken);
}
