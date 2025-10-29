using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

public record BookReferenceResponse(Guid Id, string Title, int Year)
{
    public BookReferenceResponse(BookEntity book) : this (book.Id, book.Title, book.Year)
    {
        
    }
};
