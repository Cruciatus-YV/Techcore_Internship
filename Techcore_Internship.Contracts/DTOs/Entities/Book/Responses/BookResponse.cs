using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

public record BookResponse(Guid Id, string Title, int Year, List<AuthorReferenceResponse> Authors)
{
    public BookResponse(BookEntity book)
        : this (book.Id, book.Title, book.Year, book.Authors.Select(author => new AuthorReferenceResponse(author)).ToList())
    {
        
    }
};