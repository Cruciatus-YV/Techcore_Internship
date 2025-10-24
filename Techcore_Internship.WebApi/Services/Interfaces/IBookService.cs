using Techcore_Internship.WebApi.Dto;

namespace Techcore_Internship.WebApi.Services.Interfaces;

public interface IBookService
{
    List<BookDto> GetAll();
    BookDto? Get(Guid id);
    BookDto Create(BookDto book);
    bool Update(BookDto request);
    bool UpdateTitle(Guid id, string request);
    bool Delete(Guid id);
}