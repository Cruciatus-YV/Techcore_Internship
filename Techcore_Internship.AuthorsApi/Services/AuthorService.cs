using Techcore_Internship.AuthorsApi.Data.Interfaces;
using Techcore_Internship.AuthorsApi.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;
using Techcore_Internship.Data.Cache.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.AuthorsApi.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IRedisCacheService _cache;

    public AuthorService(IAuthorRepository authorRepository, IRedisCacheService cache)
    {
        _authorRepository = authorRepository;
        _cache = cache;
    }

    public async Task<List<AuthorResponse>?> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"authors_batch_{string.Join("_", requestedIds.OrderBy(id => id))}";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var authors = await _authorRepository.GetByIdsAsync(requestedIds, cancellationToken);
                return authors?.Select(a => new AuthorResponse(a)).ToList();
            },
            TimeSpan.FromMinutes(15));
    }

    public async Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"author_{id}";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
                return author == null ? null : new AuthorResponse(author);
            },
            TimeSpan.FromMinutes(30));
    }

    public async Task<List<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default, bool includeBooks = false)
    {
        var cacheKey = includeBooks ? "authors_all_with_books" : "authors_all";

        return await _cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var authors = await _authorRepository.GetAllAsync(includeBooks, cancellationToken);
                return authors.Select(a => new AuthorResponse(a)).ToList();
            },
            TimeSpan.FromMinutes(10));
    }

    public async Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsDeleted = false
        };

        await _authorRepository.InsertEntityAsync(author, cancellationToken);

        await _cache.RemoveAsync("authors_all");
        await _cache.RemoveAsync("authors_all_with_books");

        return new AuthorResponse(author);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateAuthorInfoRequest request, CancellationToken cancellationToken = default)
    {
        var existingAuthor = await _authorRepository.GetEntityByIdAsync(id, cancellationToken);
        if (existingAuthor == null || existingAuthor.IsDeleted)
            return false;

        existingAuthor.FirstName = request.FirstName;
        existingAuthor.LastName = request.LastName;

        var result = await _authorRepository.UpdateEntityAsync(existingAuthor, cancellationToken);

        if (result)
        {
            await _cache.RemoveAsync($"author_{id}");
            await _cache.RemoveAsync("authors_all");
            await _cache.RemoveAsync("authors_all_with_books");
        }

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetEntityByIdAsync(id, cancellationToken);
        if (author == null || author.IsDeleted)
            return false;

        author.IsDeleted = true;
        var result = await _authorRepository.UpdateEntityAsync(author, cancellationToken);

        if (result)
        {
            await _cache.RemoveAsync($"author_{id}");
            await _cache.RemoveAsync("authors_all");
            await _cache.RemoveAsync("authors_all_with_books");
        }

        return result;
    }

    public async Task<bool> IsExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await GetByIdAsync(id, cancellationToken);
        return author != null;
    }
}