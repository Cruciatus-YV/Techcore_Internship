using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IBookService
{
    Task<BookResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<BookResponseDto>?> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);
    Task<List<BookResponseDto>?> GetAllWithAuthorsAsync(CancellationToken cancellationToken);
    Task<List<BookResponseDto>?> GetByYearAsync(int year, CancellationToken cancellationToken);
    Task<BookResponseDto> CreateAsync(CreateBookRequestDto request, CancellationToken cancellationToken);
    Task<Guid> CreateWithAuthorsAsync(CreateBookWithAuthorsRequestDto request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid bookId, UpdateBookRequestDto request, CancellationToken cancellationToken);
    Task<bool> UpdateAuthorsAsync(Guid id, UpdateBookAuthorsRequestDto request, CancellationToken cancellationToken);
    Task<bool> UpdateBookInfoAsync(Guid bookId, UpdateBookInfoRequestDto request, CancellationToken cancellationToken);
    Task<bool> UpdateTitleAsync(Guid bookId, string title, CancellationToken cancellationToken);
    Task<bool> UpdateYearAsync(Guid bookId, int year, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken);
}
