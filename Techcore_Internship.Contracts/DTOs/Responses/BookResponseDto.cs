using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Responses;

public record BookResponseDto(Guid Id, string Title, int Year, List<AuthorReferenceResponseDto> Authors)
{
    public BookResponseDto(BookEntity book)
        : this (book.Id, book.Title, book.Year, book.Authors.Select(author => new AuthorReferenceResponseDto(author)).ToList())
    {
        
    }
};