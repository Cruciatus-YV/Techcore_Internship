using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.Configurations;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Responses;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Cache.Interfaces;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Context.Books;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorHttpService _authorHttpService;
    private readonly IBookDapperRepository _bookDapperRepository;
    private readonly IRedisCacheService _cache;
    private readonly IDistributedCache _distributedCache;
    private readonly IProductReviewRepository _productReviewRepository;
    private readonly IOptions<RedisSettings> _redisSettings;
    private readonly ApplicationDbContext _dbContext;

    public BookService(IBookRepository bookRepository,
                       ApplicationDbContext dbContext,
                       IAuthorHttpService authorHttpService,
                       IBookDapperRepository bookDapperRepository,
                       IRedisCacheService cache,
                       IProductReviewRepository productReviewRepository,
                       IOptions<RedisSettings> redisSettings,
                       IDistributedCache distributedCache)
    {
        _bookRepository = bookRepository;
        _dbContext = dbContext;
        _authorHttpService = authorHttpService;
        _bookDapperRepository = bookDapperRepository;
        _cache = cache;
        _productReviewRepository = productReviewRepository;
        _redisSettings = redisSettings;
        _distributedCache = distributedCache;
    }

    public async Task<BookResponse?> GetByIdOutputCacheTestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
        if (book == null) return null;

        var authorIds = book.Authors?.Select(a => a.Id).ToList() ?? new List<Guid>();
        var authors = await _authorHttpService.GetByIdsAsync(authorIds, cancellationToken) ?? new List<AuthorResponse>();

        return CreateBookResponse(book, authors);
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"book_{id}";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var bookEntity = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
                if (bookEntity == null) return null;

                var authorIds = bookEntity.Authors?.Select(a => a.Id).ToList() ?? new List<Guid>();
                var authors = await _authorHttpService.GetByIdsAsync(authorIds, cancellationToken) ?? new List<AuthorResponse>();

                return CreateBookResponse(bookEntity, authors);
            },
            TimeSpan.FromMinutes(30));
    }

    public async Task<List<BookResponse>?> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books_author_{authorId}";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var bookEntities = await _bookRepository.GetByAuthorIdAsync(authorId, cancellationToken);
                if (bookEntities == null) return null;

                var result = new List<BookResponse>();
                foreach (var book in bookEntities)
                {
                    var authorIds = book.Authors?.Select(a => a.Id).ToList() ?? new List<Guid>();
                    var authors = await _authorHttpService.GetByIdsAsync(authorIds, cancellationToken) ?? new List<AuthorResponse>();
                    result.Add(CreateBookResponse(book, authors));
                }
                return result;
            },
            TimeSpan.FromMinutes(15));
    }

    public async Task<BookResponse?> GetByIdWithDapperAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"book_dapper_{id}";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () => await _bookDapperRepository.GetByIdWithAuthorsAsync(id, cancellationToken),
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

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var bookEntities = await _bookRepository.GetAllWithAuthorsAsync(cancellationToken);
                if (bookEntities == null) return null;

                var result = new List<BookResponse>();
                foreach (var book in bookEntities)
                {
                    var authorIds = book.Authors?.Select(a => a.Id).ToList() ?? new List<Guid>();
                    var authors = await _authorHttpService.GetByIdsAsync(authorIds, cancellationToken) ?? new List<AuthorResponse>();
                    result.Add(CreateBookResponse(book, authors));
                }
                return result;
            },
            TimeSpan.FromMinutes(10));
    }

    public async Task<List<BookResponse>?> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books_year_{year}";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var bookEntities = await _bookRepository.GetByYearAsync(year, cancellationToken);
                if (bookEntities == null) return null;

                var result = new List<BookResponse>();
                foreach (var book in bookEntities)
                {
                    var authorIds = book.Authors?.Select(a => a.Id).ToList() ?? new List<Guid>();
                    var authors = await _authorHttpService.GetByIdsAsync(authorIds, cancellationToken) ?? new List<AuthorResponse>();
                    result.Add(CreateBookResponse(book, authors));
                }
                return result;
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
            // Валидируем авторов через IAuthorHttpService
            var authors = await _authorHttpService.GetByIdsAsync(request.AuthorIds, cancellationToken);
            if (authors?.Count != request.AuthorIds.Count)
                throw new ArgumentException("Некорректный список AuthorIds");

            var authorEntities = authors.Select(a => new AuthorEntity
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                IsDeleted = false
            }).ToList();

            var newBook = new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Year = request.Year,
                IsDeleted = false,
                Authors = authorEntities
            };

            await _bookRepository.InsertEntityAsync(newBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await ClearBookCaches(newBook, request.AuthorIds);

            return CreateBookResponse(newBook, authors);
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
            var allAuthorIds = new List<Guid>();

            // Создаем новых авторов через IAuthorHttpService
            if (request.NewAuthors?.Any() == true)
            {
                foreach (var newAuthorDto in request.NewAuthors)
                {
                    var createdAuthor = await _authorHttpService.CreateAsync(newAuthorDto, cancellationToken);
                    allAuthorIds.Add(createdAuthor.Id);
                }
            }

            // Добавляем существующих авторов
            if (request.ExistingAuthorIds?.Any() == true)
            {
                var existingAuthors = await _authorHttpService.GetByIdsAsync(request.ExistingAuthorIds, cancellationToken);
                if (existingAuthors?.Count != request.ExistingAuthorIds.Count)
                    throw new ArgumentException("Некорректный список ExistingAuthorsIds");

                allAuthorIds.AddRange(request.ExistingAuthorIds);
            }

            if (!allAuthorIds.Any())
                throw new ArgumentException("Книга должна иметь хотя бы одного автора");

            // Получаем полные данные авторов
            var allAuthors = await _authorHttpService.GetByIdsAsync(allAuthorIds, cancellationToken) ?? new List<AuthorResponse>();
            var authorEntities = allAuthors.Select(a => new AuthorEntity
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                IsDeleted = false
            }).ToList();

            var newBook = new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Year = request.Year,
                IsDeleted = false,
                Authors = authorEntities
            };

            await _bookRepository.InsertEntityAsync(newBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await ClearBookCaches(newBook, allAuthorIds);

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

            var oldAuthorIds = existingBook.Authors.Select(a => a.Id).ToList();
            var oldYear = existingBook.Year;

            existingBook.Title = request.Title;
            existingBook.Year = request.Year;

            if (request.NewAuthors?.Any() == true || request.ExistingAuthorIds?.Any() == true)
            {
                var updatedAuthorIds = await ProcessAuthorsViaHttpAsync(request.NewAuthors, request.ExistingAuthorIds, cancellationToken);
                var updatedAuthors = await _authorHttpService.GetByIdsAsync(updatedAuthorIds, cancellationToken) ?? new List<AuthorResponse>();

                existingBook.Authors = updatedAuthors.Select(a => new AuthorEntity
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    IsDeleted = false
                }).ToList();
            }

            await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await ClearBookCaches(existingBook, oldAuthorIds);

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

            var oldAuthorIds = existingBook.Authors.Select(a => a.Id).ToList();

            if (request.NewAuthors == null && request.ExistingAuthorIds == null)
                throw new ArgumentException("Книга не может быть без авторов");

            var updatedAuthorIds = await ProcessAuthorsViaHttpAsync(request.NewAuthors, request.ExistingAuthorIds, cancellationToken);
            var updatedAuthors = await _authorHttpService.GetByIdsAsync(updatedAuthorIds, cancellationToken) ?? new List<AuthorResponse>();

            existingBook.Authors = updatedAuthors.Select(a => new AuthorEntity
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                IsDeleted = false
            }).ToList();

            await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await ClearBookCaches(existingBook, oldAuthorIds);

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

            var authorIds = existingBook.Authors?.Select(a => a.Id).ToList() ?? new List<Guid>();
            var year = existingBook.Year;

            existingBook.IsDeleted = true;
            await _bookRepository.UpdateEntityAsync(existingBook, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await ClearBookCaches(existingBook, authorIds);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> DeleteForeverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var requestedBook = await _bookRepository.GetByIdWithAuthorsAsync(id, cancellationToken);
        if (requestedBook == null)
            return false;

        var authorIds = requestedBook.Authors.Select(a => a.Id).ToList();
        var year = requestedBook.Year;

        requestedBook.Authors.Clear();
        await _bookRepository.UpdateEntityAsync(requestedBook, cancellationToken);

        await _bookRepository.DeleteEntityAsync(requestedBook, cancellationToken);

        await ClearBookCaches(requestedBook, authorIds);

        return true;
    }

    public async Task<ProductDetailsResponse?> GetProductDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"{_redisSettings.Value.InstanceName}average_book_{id}_rating";
        var bookTask = GetByIdAsync(id, cancellationToken);
        var reviewsTask = _productReviewRepository.GetByProductIdAsync(id, cancellationToken);
        var avgBookRatingTask = _distributedCache.GetStringAsync(key, cancellationToken);
        await Task.WhenAll(bookTask, reviewsTask, avgBookRatingTask);

        var book = bookTask.Result;
        var reviews = reviewsTask.Result;
        var avgBookRating = avgBookRatingTask.Result;

        return book == null
            ? null
            : new ProductDetailsResponse(book, reviews?.Select(r => new ProductReviewResponse(r))?.ToList() ?? [], avgBookRating ?? "No rating yet");
    }

    private async Task<List<Guid>> ProcessAuthorsViaHttpAsync(List<CreateAuthorRequest>? newAuthors, List<Guid>? existingAuthorIds, CancellationToken cancellationToken)
    {
        var authorIds = new List<Guid>();

        if (newAuthors?.Any() == true)
        {
            foreach (var newAuthorDto in newAuthors)
            {
                var createdAuthor = await _authorHttpService.CreateAsync(newAuthorDto, cancellationToken);
                authorIds.Add(createdAuthor.Id);
            }
        }

        if (existingAuthorIds?.Any() == true)
        {
            var existingAuthors = await _authorHttpService.GetByIdsAsync(existingAuthorIds, cancellationToken);
            if (existingAuthors?.Count != existingAuthorIds.Count)
                throw new ArgumentException("Некорректный список ExistingAuthorIds");

            authorIds.AddRange(existingAuthorIds);
        }

        return authorIds;
    }

    private BookResponse CreateBookResponse(BookEntity book, List<AuthorResponse> authors)
    {
        return new BookResponse(
            book.Id,
            book.Title,
            book.Year,
            authors.Select(a => new AuthorReferenceResponse(a.Id, a.FirstName, a.LastName)).ToList()
        );
    }

    private async Task ClearBookCaches(BookEntity book, List<Guid> authorIds)
    {
        await _cache.RemoveAsync($"book_{book.Id}");
        await _cache.RemoveAsync("books_all_ef");
        await _cache.RemoveAsync("books_all_dapper");
        foreach (var authorId in authorIds)
            await _cache.RemoveAsync($"books_author_{authorId}");
        await _cache.RemoveAsync($"books_year_{book.Year}");
    }
}