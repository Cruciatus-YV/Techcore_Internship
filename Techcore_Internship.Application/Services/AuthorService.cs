using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Techcore_Internship.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ApplicationDbContext _dbContext;

    public AuthorService(
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        ApplicationDbContext dbContext)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _dbContext = dbContext;
    }

    public async Task<List<AuthorResponseDto>?> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken)
    {
        var authors = await _authorRepository.GetByIdsAsync(requestedIds, cancellationToken);

        return authors?.Select(a => new AuthorResponseDto(a)).ToList();
    }
}