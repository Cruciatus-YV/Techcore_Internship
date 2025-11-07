using Techcore_Internship.AuthorsApi.Contracts.DTOs.Requests;
using Techcore_Internship.AuthorsApi.Contracts.DTOs.Responses;
using Techcore_Internship.AuthorsApi.Data.Interfaces;
using Techcore_Internship.AuthorsApi.Domain;
using Techcore_Internship.AuthorsApi.Services.Interfaces;

namespace Techcore_Internship.AuthorsApi.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;
    }

    public async Task<AuthorReferenceResponse> CreateAsync(CreateAuthorRequest request)
    {
        _logger.LogInformation("Creating new author: {FirstName} {LastName}",
            request.FirstName, request.LastName);

        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsDeleted = false
        };

        var createdId = await _authorRepository.CreateAsync(author);

        _logger.LogInformation("Author created with ID: {AuthorId}", createdId);

        return new AuthorReferenceResponse(author);
    }

    public async Task<AuthorReferenceResponse> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting author by ID: {AuthorId}", id);

        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
        {
            _logger.LogWarning("Author with ID {AuthorId} not found", id);
            throw new AuthorNotFoundException(id);
        }

        return new AuthorReferenceResponse(author);
    }

    public async Task<List<AuthorReferenceResponse>> GetAllAsync()
    {
        _logger.LogInformation("Getting all authors");

        var authors = await _authorRepository.GetAllAsync();
        return authors.Select(author => new AuthorReferenceResponse(author)).ToList();
    }


    public async Task<AuthorReferenceResponse> UpdateAsync(Guid id, UpdateAuthorInfoRequest request)
    {
        _logger.LogInformation("Updating author with ID: {AuthorId}", id);

        var existingAuthor = await _authorRepository.GetByIdAsync(id);
        if (existingAuthor == null)
        {
            _logger.LogWarning("Author with ID {AuthorId} not found for update", id);
            throw new AuthorNotFoundException(id);
        }

        var updatedAuthor = new AuthorEntity
        {
            Id = existingAuthor.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsDeleted = existingAuthor.IsDeleted
        };

        var success = await _authorRepository.UpdateAsync(updatedAuthor);
        if (!success)
        {
            _logger.LogError("Failed to update author with ID: {AuthorId}", id);
            throw new Exception($"Failed to update author with ID {id}");
        }

        _logger.LogInformation("Author with ID {AuthorId} updated successfully", id);

        return new AuthorReferenceResponse(updatedAuthor);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting author with ID: {AuthorId}", id);

        var success = await _authorRepository.DeleteAsync(id);
        if (!success)
        {
            _logger.LogWarning("Author with ID {AuthorId} not found for deletion", id);
            return false;
        }

        _logger.LogInformation("Author with ID {AuthorId} deleted successfully", id);
        return true;
    }
}
public class AuthorNotFoundException : Exception
{
    public AuthorNotFoundException(Guid authorId)
        : base($"Author with ID {authorId} not found") { }
}