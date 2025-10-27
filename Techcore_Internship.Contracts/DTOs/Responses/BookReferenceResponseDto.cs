using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Responses;

public record BookReferenceResponseDto(Guid Id, string Title, int Year)
{
    public BookReferenceResponseDto(BookEntity book) : this (book.Id, book.Title, book.Year)
    {
        
    }
};
