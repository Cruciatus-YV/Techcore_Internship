using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs;

namespace Techcore_Internship.Application.Services;

public class BookService : IBookService
{
    private static readonly List<BookDto> _books =
    [
        new BookDto(Guid.NewGuid(), "Book One", "Author One", 1950),
    new BookDto(Guid.NewGuid(), "Book Two", "Author Two", 1960)
    ];

    public async Task<List<BookDto>> GetAll()
    {
        await Task.Delay(100);
        return _books;
    }

    public async Task<BookDto?> Get(Guid id)
    {
        await Task.Delay(100);
        return _books.FirstOrDefault(b => b.Id == id);
    }

    public async Task<BookDto> Create(CreateBookDto book)
    {
        await Task.Delay(100);
        var newBook = new BookDto(Guid.NewGuid(), book.Title, book.Author, book.Year);

        _books.Add(newBook);

        return newBook;
    }

    public async Task<bool> Update(BookDto request)
    {
        await Task.Delay(100);
        var existingBookIndex = _books.FindIndex(b => b.Id == request.Id);
        if (existingBookIndex < 0)
            return false;

        _books[existingBookIndex] = request;

        return true;
    }

    public async Task<bool> UpdateTitle(Guid id, string request)
    {
        await Task.Delay(100);
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

    public async Task<bool> Delete(Guid id)
    {
        await Task.Delay(100);
        var existingBookIndex = _books.FindIndex(b => b.Id == id);
        if (existingBookIndex < 0)
            return false;

        _books.RemoveAt(existingBookIndex);

        return true;
    }
}
