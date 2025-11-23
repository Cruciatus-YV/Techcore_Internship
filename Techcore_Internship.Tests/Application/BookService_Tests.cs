using Moq;
using Techcore_Internship.Application.Services.Context.Books;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Data.Cache.Interfaces;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.UnitTests.Application;

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
            null, null, null, cacheServiceMock.Object, null, null, null, null
        );

        // Act
        var result = await bookService.GetByIdAsync(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookId, result.Id);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task DeleteForeverAsync_BookExists_CallsDeleteEntityAsync()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var existingBook = new BookEntity
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
            .ReturnsAsync(existingBook);

        bookRepositoryMock
            .Setup(r => r.UpdateEntityAsync(It.IsAny<BookEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        bookRepositoryMock
            .Setup(r => r.DeleteEntityAsync(It.IsAny<BookEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        cacheServiceMock
            .Setup(c => c.RemoveAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var bookService = new BookService(
            bookRepositoryMock.Object,
            null, null, null, cacheServiceMock.Object, null, null, null, null
        );

        // Act
        var result = await bookService.DeleteForeverAsync(bookId);

        // Assert
        Assert.True(result);

        bookRepositoryMock.Verify(
            r => r.DeleteEntityAsync(
                It.Is<BookEntity>(book => book.Id == bookId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}