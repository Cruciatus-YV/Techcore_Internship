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
    private readonly IAuthorRepository _authorRepository;
    private readonly ApplicationDbContext _dbContext;

    public BookService(IBookRepository bookRepository, ApplicationDbContext dbContext, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _dbContext = dbContext;
        _authorRepository = authorRepository;
    }

    public async Task<BookResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
        return (book == null) ? null : new BookResponseDto(book);
    }

    public async Task<List<BookResponseDto>?> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetByAuthorIdAsync(authorId, cancellationToken);
        return books?.Select(b => new BookResponseDto(b)).ToList();
    }

    public async Task<List<BookResponseDto>?> GetAllWithAuthorsAsync(CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllWithAuthorsAsync(cancellationToken);
        return books?.Select(b => new BookResponseDto(b)).ToList();
    }

    public async Task<List<BookResponseDto>?> GetByYearAsync(int year, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetByYearAsync(year, cancellationToken);
        return books?.Select(b => new BookResponseDto(b)).ToList();
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookRequestDto request, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var foundedAuthors = await _authorRepository.GetByIdsAsync(request.AuthorIds, cancellationToken);

            if (foundedAuthors.Count != request.AuthorIds.Count)
            {
                throw new ArgumentException("Некорректный список AuthorIds");
            }

            var newBook = new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Year = request.Year,
                IsDeleted = false,
                Authors = foundedAuthors
            };

            await _bookRepository.InsertAsync(newBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new BookResponseDto(newBook);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Guid> CreateWithAuthorsAsync(CreateBookWithAuthorsRequestDto request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var newAuthors = request.NewAuthors.Select(newAuthorDto => new AuthorEntity
            {
                Id = Guid.NewGuid(),
                FirstName = newAuthorDto.FirstName,
                LastName = newAuthorDto.LastName,
                IsDeleted = false
            }).ToList();

            await _authorRepository.InsertListAsync(newAuthors, cancellationToken);

            var existingAuthors = new List<AuthorEntity>();
            if (request.ExistingAuthorIds?.Any() == true)
            {
                existingAuthors = await _authorRepository.GetByIdsAsync(request.ExistingAuthorIds, cancellationToken);

                if (existingAuthors.Count != request.ExistingAuthorIds.Count)
                {
                    throw new ArgumentException("Некорректный список ExistingAuthorsIds");
                }
            }

            var allAuthors = newAuthors.Concat(existingAuthors).ToList();

            if (!allAuthors.Any())
            {
                throw new ArgumentException("Книга должна иметь хотя бы одного автора");
            }

            var newBook = new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Year = request.Year,
                IsDeleted = false,
                Authors = allAuthors
            };

            await _bookRepository.InsertAsync(newBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return newBook.Id;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Guid bookId, UpdateBookRequestDto request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingBook = await _bookRepository.GetByIdWithAuthorsAsync(bookId, cancellationToken);
            if (existingBook == null || existingBook.IsDeleted)
                return false;

            existingBook.Title = request.Title;
            existingBook.Year = request.Year;

            if (request.NewAuthors?.Any() == true || request.ExistingAuthorIds?.Any() == true)
            {
                var updatedAuthors = await GetUpdatedAuthorsAsync(request.NewAuthors, request.ExistingAuthorIds, cancellationToken);
                existingBook.Authors = updatedAuthors;
            }

            await _bookRepository.UpdateAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAuthorsAsync(Guid id, UpdateBookAuthorsRequestDto request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingBook = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
            if (existingBook == null || existingBook.IsDeleted)
                throw new ArgumentException("Книга не найдена");

            if (request.NewAuthors == null && request.ExistingAuthorIds == null)
            {
                throw new ArgumentException("Книга не может быть без авторов");
            }
            else
            {
                var updatedAuthors = await GetUpdatedAuthorsAsync(request.NewAuthors, request.ExistingAuthorIds, cancellationToken);
                existingBook.Authors = updatedAuthors;
            }

            await _bookRepository.UpdateAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateBookInfoAsync(Guid bookId, UpdateBookInfoRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
        if (existingBook == null || existingBook.IsDeleted)
            return false;

        existingBook.Title = request.Title;
        existingBook.Year = request.Year;
        return await _bookRepository.UpdateAsync(existingBook, cancellationToken);
    }

    public async Task<bool> UpdateTitleAsync(Guid bookId, string title, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
        if (existingBook == null || existingBook.IsDeleted)
            return false;

        existingBook.Title = title;
        return await _bookRepository.UpdateAsync(existingBook, cancellationToken);
    }

    public async Task<bool> UpdateYearAsync(Guid bookId, int year, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
        if (existingBook == null || existingBook.IsDeleted)
            return false;

        existingBook.Year = year;
        return await _bookRepository.UpdateAsync(existingBook, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingBook = await _bookRepository.GetByIdAsync(id, cancellationToken);

            if (existingBook == null)
                return false;

            if (existingBook.IsDeleted)
            {
                return true;
            }

            existingBook.IsDeleted = true;

            await _bookRepository.UpdateAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken)
    {
        return await _bookRepository.Exist(id, cancellationToken);
    }

    private async Task<List<AuthorEntity>> GetUpdatedAuthorsAsync(List<CreateAuthorRequestDto>? newAuthors,
                                                                  List<Guid>? existingAuthorIds,
                                                                  CancellationToken cancellationToken)
    {
        var authors = new List<AuthorEntity>();

        if (newAuthors?.Any() == true)
        {
            var newAuthorEntities = newAuthors.Select(newAuthorDto => new AuthorEntity
            {
                Id = Guid.NewGuid(),
                FirstName = newAuthorDto.FirstName,
                LastName = newAuthorDto.LastName,
                IsDeleted = false
            }).ToList();

            await _authorRepository.InsertListAsync(newAuthorEntities, cancellationToken);
            authors.AddRange(newAuthorEntities);
        }

        if (existingAuthorIds?.Any() == true)
        {
            var existingAuthors = await _authorRepository.GetByIdsAsync(existingAuthorIds, cancellationToken);

            if (existingAuthors.Count != existingAuthorIds.Count)
                throw new ArgumentException("Некорректный список ExistingAuthorIds");

            authors.AddRange(existingAuthors);
        }

        return authors;
    }
}