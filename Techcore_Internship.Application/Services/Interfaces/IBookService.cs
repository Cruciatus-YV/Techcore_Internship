using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IBookService
{
    Task<List<BookResponseDto>> GetAll();
    Task<BookResponseDto?> Get(Guid id);
    Task<BookResponseDto> Create(CreateBookRequestDto book);
    Task<bool> Update(BookResponseDto request);
    Task<bool> UpdateTitle(Guid id, string request);
    Task<bool> Delete(Guid id);
}
