using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<List<AuthorResponse>?> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default)
    {
        var authors = await _authorRepository.GetByIdsAsync(requestedIds, cancellationToken);

        return authors?.Select(a => new AuthorResponse(a)).ToList();
    }

    public async Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);

        return author == null 
            ? null 
            : new AuthorResponse(author);
    }

    public async Task<List<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default, bool includeBooks = false)
    {
        var authors = await _authorRepository.GetAllAsync(includeBooks, cancellationToken);

        return authors.Select(a => new AuthorResponse(a)).ToList();
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
        return new AuthorResponse(author);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateAuthorInfoRequest request, CancellationToken cancellationToken = default)
    {
        var existingAuthor = await _authorRepository.GetEntityByIdAsync(id, cancellationToken);
        if (existingAuthor == null || existingAuthor.IsDeleted)
            return false;

        existingAuthor.FirstName = request.FirstName;
        existingAuthor.LastName = request.LastName;

        return await _authorRepository.UpdateEntityAsync(existingAuthor, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetEntityByIdAsync(id, cancellationToken);
        if (author == null || author.IsDeleted)
            return false;

        author.IsDeleted = true;
        return await _authorRepository.UpdateEntityAsync(author, cancellationToken);
    }

    public async Task<bool> IsExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _authorRepository.IsEntityExists(id, cancellationToken);
    }
}