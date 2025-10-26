using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ApplicationDbContext _dbContext;

    public BookService(IBookRepository bookRepository, ApplicationDbContext dbContext)
    {
        _bookRepository = bookRepository;
        _dbContext = dbContext;
    }

    public async Task<List<BookResponseDto>> GetAll()
    {
        var books = await _bookRepository.GetAllAsync();

        return books.Select(b => new BookResponseDto(b.Id, b.Title, b.Year, b.IsDeleted, b.AuthorId)).ToList();
    }

    public async Task<BookResponseDto?> Get(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return (book == null || book.IsDeleted) 
            ? null 
            : new BookResponseDto(book.Id, book.Title, book.Year, book.IsDeleted, book.AuthorId);
    }

    public async Task<BookResponseDto> Create(CreateBookRequestDto book)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var newBook = new BookEntity
            {
                Id = Guid.NewGuid(),
                Title = book.Title,
                Year = book.Year,
                IsDeleted = book.IsDeleted,
                AuthorId = book.AuthorId
            };

            await _bookRepository.InsertAsync(newBook);
            await transaction.CommitAsync();

            return new BookResponseDto(newBook.Id, newBook.Title, newBook.Year, newBook.IsDeleted, newBook.AuthorId);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> Update(BookResponseDto request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var existingBook = await _bookRepository.GetByIdAsync(request.Id);

            if (existingBook == null)
                return false;

            existingBook.Title = request.Title;
            existingBook.Year = request.Year;
            existingBook.IsDeleted = request.IsDeleted;
            existingBook.AuthorId = request.AuthorId;

            await _bookRepository.UpdateAsync(existingBook);
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateTitle(Guid id, string title)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);

            if (existingBook == null || existingBook.IsDeleted)
                return false;

            existingBook.Title = title;

            await _bookRepository.UpdateAsync(existingBook);
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> Delete(Guid id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);

            if (existingBook == null)
                return false;

            if (existingBook.IsDeleted)
            {
                return true; 
            }

            existingBook.IsDeleted = true;

            await _bookRepository.UpdateAsync(existingBook);
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> Exists(Guid id)
    {
        return await _bookRepository.Exist(id);
    }

    public async Task<List<BookResponseDto>> GetBooksByAuthor(Guid authorId)
    {
        var books = await _bookRepository.GetAllAsync();
        return books
            .Where(b => b.AuthorId == authorId && !b.IsDeleted)
            .Select(b => new BookResponseDto(b.Id, b.Title, b.Year, b.IsDeleted, b.AuthorId))
            .ToList();
    }

    public async Task<List<BookResponseDto>> GetBooksByYear(int year)
    {
        var books = await _bookRepository.GetAllAsync();
        return books
            .Where(b => b.Year == year && !b.IsDeleted)
            .Select(b => new BookResponseDto(b.Id, b.Title, b.Year, b.IsDeleted, b.AuthorId))
            .ToList();
    }
}