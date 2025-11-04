using Moq;
using Xunit;
using Techcore_Internship.Application.Services.Context.Books;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Domain.Entities;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

namespace Techcore_Internship.Tests.Unit.Application;

public class BookService_Tests
{
    [Fact]
    public async Task GetByIdAsync_BookExists_ReturnsBookResponse()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var bookEntity = new BookEntity
        {
            Id = bookId,
            Title = "Test Book",
            Year = 2023,
            Authors = new List<AuthorEntity>()
        };

        var bookRepositoryMock = new Mock<IBookRepository>();
        var cacheServiceMock = new Mock<IRedisCacheService>();

        bookRepositoryMock
            .Setup(r => r.GetByIdWithAuthorsAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookEntity);

        cacheServiceMock
            .Setup(c => c.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<BookResponse?>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync((string key, Func<Task<BookResponse?>> factory, TimeSpan? expiration) =>
                factory().Result
            );

        var bookService = new BookService(
            bookRepositoryMock.Object,
            null, null, null, cacheServiceMock.Object, null, null, null
        );

        // Act
        var result = await bookService.GetByIdAsync(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookId, result.Id);
        Assert.Equal("Test Book", result.Title);
    }
}
