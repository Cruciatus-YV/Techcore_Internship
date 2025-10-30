using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Entities;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookDapperRepository _bookDapperRepository;
    private readonly IRedisCacheService _cache;
    private readonly IProductReviewRepository _productReviewRepository;
    private readonly ApplicationDbContext _dbContext;

    public BookService(IBookRepository bookRepository,
                       ApplicationDbContext dbContext,
                       IAuthorRepository authorRepository,
                       IBookDapperRepository bookDapperRepository,
                       IRedisCacheService cache,
                       IProductReviewRepository productReviewRepository)
    {
        _bookRepository = bookRepository;
        _dbContext = dbContext;
        _authorRepository = authorRepository;
        _bookDapperRepository = bookDapperRepository;
        _cache = cache;
        _productReviewRepository = productReviewRepository;
    }
    public async Task<BookResponse?> GetByIdOutputCacheTestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
        return book == null ? null : new BookResponse(book);
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"book_{id}";

        return await _cache.GetOrCreateAsync<BookResponse?>(cacheKey,
            async () =>
            {
                var bookEntity = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
                return bookEntity == null ? null : new BookResponse(bookEntity);
            },
            TimeSpan.FromMinutes(30));
    }

    public async Task<List<BookResponse>?> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books_author_{authorId}";

        return await _cache.GetOrCreateAsync<List<BookResponse>?>(cacheKey,
            async () =>
            {
                var bookEntities = await _bookRepository.GetByAuthorIdAsync(authorId, cancellationToken);
                return bookEntities?.Select(b => new BookResponse(b)).ToList();
            },
            TimeSpan.FromMinutes(15));
    }

    public async Task<BookResponse?> GetByIdWithDapperAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"book_dapper_{id}";

        return await _cache.GetOrCreateAsync<BookResponse?>(cacheKey,
            async () =>  await _bookDapperRepository.GetByIdWithAuthorsAsync(id, cancellationToken),
            TimeSpan.FromMinutes(30));
    }

    public async Task<List<BookResponse>?> GetAllWithAuthorsFromDapperAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = "books_all_dapper";

        return await _cache.GetOrCreateAsync<List<BookResponse>?>(cacheKey,
            async () => await _bookDapperRepository.GetAllWithAuthorsAsync(cancellationToken),
            TimeSpan.FromMinutes(10));
    }

    public async Task<List<BookResponse>?> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = "books_all_ef";

        return await _cache.GetOrCreateAsync<List<BookResponse>?>(cacheKey,
            async () =>
            {
                var bookEntities = await _bookRepository.GetAllWithAuthorsAsync(cancellationToken);
                return bookEntities?.Select(b => new BookResponse(b)).ToList();
            },
            TimeSpan.FromMinutes(10));
    }

    public async Task<List<BookResponse>?> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books_year_{year}";

        return await _cache.GetOrCreateAsync<List<BookResponse>?>(cacheKey,
            async () =>
            {
                var bookEntities = await _bookRepository.GetByYearAsync(year, cancellationToken);
                return bookEntities?.Select(b => new BookResponse(b)).ToList();
            },
            TimeSpan.FromMinutes(20));
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        return await _bookRepository.IsEntityExists(id, cancellationToken);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var foundedAuthors = await _authorRepository.GetByIdsAsync(request.AuthorIds, cancellationToken);

            if (foundedAuthors.Count != request.AuthorIds.Count)
                throw new ArgumentException("Некорректный список AuthorIds");

            var newBook = new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Year = request.Year,
                IsDeleted = false,
                Authors = foundedAuthors
            };

            await _bookRepository.InsertEntityAsync(newBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");
            foreach (var author in newBook.Authors)
                await _cache.RemoveAsync($"books_author_{author.Id}");
            await _cache.RemoveAsync($"books_year_{newBook.Year}");

            return new BookResponse(newBook);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Guid> CreateWithAuthorsAsync(CreateBookWithAuthorsRequest request, CancellationToken cancellationToken = default)
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

            await _authorRepository.InsertListEntityAsync(newAuthors, cancellationToken);

            var existingAuthors = new List<AuthorEntity>();
            if (request.ExistingAuthorIds?.Any() == true)
            {
                existingAuthors = await _authorRepository.GetByIdsAsync(request.ExistingAuthorIds, cancellationToken);
                if (existingAuthors.Count != request.ExistingAuthorIds.Count)
                    throw new ArgumentException("Некорректный список ExistingAuthorsIds");
            }

            var allAuthors = newAuthors.Concat(existingAuthors).ToList();
            if (!allAuthors.Any())
                throw new ArgumentException("Книга должна иметь хотя бы одного автора");

            var newBook = new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Year = request.Year,
                IsDeleted = false,
                Authors = allAuthors
            };

            await _bookRepository.InsertEntityAsync(newBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");
            foreach (var author in newBook.Authors)
                await _cache.RemoveAsync($"books_author_{author.Id}");
            await _cache.RemoveAsync($"books_year_{newBook.Year}");

            return newBook.Id;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Guid bookId, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingBook = await _bookRepository.GetByIdWithAuthorsAsync(bookId, cancellationToken);
            if (existingBook == null || existingBook.IsDeleted)
                return false;

            var oldAuthors = existingBook.Authors.ToList();
            var oldYear = existingBook.Year;

            existingBook.Title = request.Title;
            existingBook.Year = request.Year;

            if (request.NewAuthors?.Any() == true || request.ExistingAuthorIds?.Any() == true)
            {
                var updatedAuthors = await GetUpdatedAuthorsAsync(request.NewAuthors, request.ExistingAuthorIds, cancellationToken);
                existingBook.Authors = updatedAuthors;
            }

            await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _cache.RemoveAsync($"book_{bookId}");
            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");

            foreach (var author in oldAuthors)
                await _cache.RemoveAsync($"books_author_{author.Id}");
            foreach (var author in existingBook.Authors)
                await _cache.RemoveAsync($"books_author_{author.Id}");

            await _cache.RemoveAsync($"books_year_{oldYear}");
            await _cache.RemoveAsync($"books_year_{existingBook.Year}");

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAuthorsAsync(Guid id, UpdateBookAuthorsRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingBook = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
            if (existingBook == null || existingBook.IsDeleted)
                throw new ArgumentException("Книга не найдена");

            var oldAuthors = existingBook.Authors.ToList();

            if (request.NewAuthors == null && request.ExistingAuthorIds == null)
                throw new ArgumentException("Книга не может быть без авторов");

            var updatedAuthors = await GetUpdatedAuthorsAsync(request.NewAuthors, request.ExistingAuthorIds, cancellationToken);
            existingBook.Authors = updatedAuthors;

            await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _cache.RemoveAsync($"book_{id}");
            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");

            foreach (var author in oldAuthors)
                await _cache.RemoveAsync($"books_author_{author.Id}");
            foreach (var author in existingBook.Authors)
                await _cache.RemoveAsync($"books_author_{author.Id}");

            await _cache.RemoveAsync($"books_year_{existingBook.Year}");

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateBookInfoAsync(Guid bookId, UpdateBookInfoRequest request, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetEntityByIdAsync(bookId, cancellationToken);
        if (existingBook == null || existingBook.IsDeleted)
            return false;

        var oldYear = existingBook.Year;
        existingBook.Title = request.Title;
        existingBook.Year = request.Year;

        var result = await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);

        if (result)
        {
            await _cache.RemoveAsync($"book_{bookId}");
            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");
            await _cache.RemoveAsync($"books_year_{oldYear}");
            await _cache.RemoveAsync($"books_year_{existingBook.Year}");
        }

        return result;
    }

    public async Task<bool> UpdateTitleAsync(Guid bookId, string title, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetEntityByIdAsync(bookId, cancellationToken);
        if (existingBook == null || existingBook.IsDeleted)
            return false;

        existingBook.Title = title;

        var result = await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);

        if (result)
        {
            await _cache.RemoveAsync($"book_{bookId}");
            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");
        }

        return result;
    }

    public async Task<bool> UpdateYearAsync(Guid bookId, int year, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetEntityByIdAsync(bookId, cancellationToken);
        if (existingBook == null || existingBook.IsDeleted)
            return false;

        var oldYear = existingBook.Year;
        existingBook.Year = year;

        var result = await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);

        if (result)
        {
            await _cache.RemoveAsync($"book_{bookId}");
            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");
            await _cache.RemoveAsync($"books_year_{oldYear}");
            await _cache.RemoveAsync($"books_year_{year}");
        }

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingBook = await _bookRepository.GetEntityByIdAsync(id, cancellationToken);
            if (existingBook == null)
                return false;

            if (existingBook.IsDeleted)
                return true;

            var authors = existingBook.Authors.ToList();
            var year = existingBook.Year;

            existingBook.IsDeleted = true;
            await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _cache.RemoveAsync($"book_{id}");
            await _cache.RemoveAsync("books_all_ef");
            await _cache.RemoveAsync("books_all_dapper");
            foreach (var author in authors)
                await _cache.RemoveAsync($"books_author_{author.Id}");
            await _cache.RemoveAsync($"books_year_{year}");

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<List<AuthorEntity>> GetUpdatedAuthorsAsync(List<CreateAuthorRequest>? newAuthors,
                                                                  List<Guid>? existingAuthorIds,
                                                                  CancellationToken cancellationToken = default)
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

            await _authorRepository.InsertListEntityAsync(newAuthorEntities, cancellationToken);
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

    public async Task<ProductDetailsResponse?> GetProductDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var bookTask = GetByIdAsync(id, cancellationToken);
        var reviewsTask = _productReviewRepository.GetByProductIdAsync(id, cancellationToken);

        await Task.WhenAll(bookTask, reviewsTask);

        var book = bookTask.Result;
        var reviews = reviewsTask.Result;

        return book == null 
            ? null 
            : new ProductDetailsResponse(book, reviews?.Select(r => new ProductReviewResponse(r))?.ToList() ?? []);
    }
}