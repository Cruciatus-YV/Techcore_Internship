using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ApplicationDbContext _dbContext;

    public AuthorService(IAuthorRepository authorRepository, ApplicationDbContext dbContext)
    {
        _authorRepository = authorRepository;
        _dbContext = dbContext;
    }

    public async Task<List<AuthorResponseDto>> GetAll()
    {
        var authors = await _authorRepository.GetAllAsync();

        return authors.Select(a => new AuthorResponseDto(a.Id, a.FirstName, a.LastName, a.IsDeleted)).ToList();
    }

    public async Task<AuthorResponseDto?> Get(Guid id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        return (author == null || author.IsDeleted)
            ? null
            : new AuthorResponseDto(author.Id, author.FirstName, author.LastName, author.IsDeleted);
    }

    public async Task<AuthorResponseDto> Create(CreateAuthorRequestDto author)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var newAuthor = new AuthorEntity
            {
                Id = Guid.NewGuid(),
                FirstName = author.FirstName,
                LastName = author.LastName,
                IsDeleted = author.IsDeleted
            };

            await _authorRepository.InsertAsync(newAuthor);
            await transaction.CommitAsync();

            return new AuthorResponseDto(newAuthor.Id, newAuthor.FirstName, newAuthor.LastName, newAuthor.IsDeleted);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> Update(AuthorResponseDto request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(request.Id);

            if (existingAuthor == null)
                return false;

            existingAuthor.FirstName = request.FirstName;
            existingAuthor.LastName = request.LastName;
            existingAuthor.IsDeleted = request.IsDeleted;

            await _authorRepository.UpdateAsync(existingAuthor);
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateName(Guid id, string firstName)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(id);

            if (existingAuthor == null || existingAuthor.IsDeleted)
                return false;

            existingAuthor.FirstName = firstName;

            await _authorRepository.UpdateAsync(existingAuthor);
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
            var existingAuthor = await _authorRepository.GetByIdAsync(id);

            if (existingAuthor == null)
                return false;

            if (existingAuthor.IsDeleted)
            {
                return true;
            }

            existingAuthor.IsDeleted = true;

            await _authorRepository.UpdateAsync(existingAuthor);
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
        return await _authorRepository.Exist(id);
    }

    public async Task<List<AuthorResponseDto>> GetAuthorsByFirstName(string firstName)
    {
        var authors = await _authorRepository.GetAllAsync();
        return authors
            .Where(a => a.FirstName.Contains(firstName) && !a.IsDeleted)
            .Select(a => new AuthorResponseDto(a.Id, a.FirstName, a.LastName, a.IsDeleted))
            .ToList();
    }

    public async Task<List<AuthorResponseDto>> GetAuthorsByLastName(string lastName)
    {
        var authors = await _authorRepository.GetAllAsync();
        return authors
            .Where(a => a.LastName.Contains(lastName) && !a.IsDeleted)
            .Select(a => new AuthorResponseDto(a.Id, a.FirstName, a.LastName, a.IsDeleted))
            .ToList();
    }
}