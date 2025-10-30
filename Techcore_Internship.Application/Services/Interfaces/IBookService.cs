using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IBookService
{
    Task<BookResponse?> GetByIdOutputCacheTestAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<BookResponse>?> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<List<BookResponse>?> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
    Task<List<BookResponse>?> GetByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<Guid> CreateWithAuthorsAsync(CreateBookWithAuthorsRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid bookId, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAuthorsAsync(Guid id, UpdateBookAuthorsRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateBookInfoAsync(Guid bookId, UpdateBookInfoRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateTitleAsync(Guid bookId, string title, CancellationToken cancellationToken = default);
    Task<bool> UpdateYearAsync(Guid bookId, int year, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    Task<List<BookResponse>?> GetAllWithAuthorsFromDapperAsync(CancellationToken cancellationToken = default);
    Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
