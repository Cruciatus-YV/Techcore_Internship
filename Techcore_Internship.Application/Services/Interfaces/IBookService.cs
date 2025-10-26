using Techcore_Internship.Contracts.DTOs;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IBookService
{
    Task<List<BookDto>> GetAll();
    Task<BookDto?> Get(Guid id);
    Task<BookDto> Create(CreateBookDto book);
    Task<bool> Update(BookDto request);
    Task<bool> UpdateTitle(Guid id, string request);
    Task<bool> Delete(Guid id);
}
