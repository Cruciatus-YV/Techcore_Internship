using Techcore_Internship.WebApi.Dto;
using Techcore_Internship.WebApi.Services.Interfaces;

namespace Techcore_Internship.WebApi.Services;

public class BookService : IBookService
{
    private static readonly List<BookDto> _books =
    [
        new BookDto(Guid.NewGuid(), "Book One", "Author One"),
        new BookDto(Guid.NewGuid(), "Book Two", "Author Two")
    ];

    public List<BookDto> GetAll()
    {
        return _books;
    }

    public BookDto? Get(Guid id)
    {
        return _books.FirstOrDefault(b => b.Id == id);
    }

    public BookDto Create(BookDto book)
    {
        var newBook = new BookDto(Guid.NewGuid(), book.Title, book.Author);

        _books.Add(newBook);

        return newBook;
    }

    public bool Update(BookDto request)
    {
        var existingBookIndex = _books.FindIndex(b => b.Id == request.Id);
        if (existingBookIndex < 0)
            return false;

        _books[existingBookIndex] = request;

        return true;
    }

    public bool UpdateTitle(Guid id, string request)
    {
        var existingBookIndex = _books.FindIndex(b => b.Id == id);
        if (existingBookIndex < 0)
            return false;

        var existingBook = _books[existingBookIndex];

        var updatedBook = existingBook with
        {
            Title = request
        };

        _books[existingBookIndex] = updatedBook;

        return true;
    }

    public bool Delete(Guid id)
    {
        var existingBookIndex = _books.FindIndex(b => b.Id == id);
        if (existingBookIndex < 0)
            return false;

        _books.RemoveAt(existingBookIndex);

        return true;
    }
}

