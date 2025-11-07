using System.Net.Http.Json;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Xunit;

public class BooksE2EPgTests
{
    [Fact]
    public async Task PostAndGet_Book_E2E_WithPostgresContainer()
    {
        // Arrange
        await using var factory = new PostgresTestFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

        var createDto = new CreateBookWithAuthorsRequest(
            "E2E Test Book",
            2023,
            new List<CreateAuthorRequest> { new("AuthorFirst", "AuthorLast") }
        );

        // Act
        var postResponse = await client.PostAsJsonAsync("api/Books/with-authors", createDto);
        postResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync("api/Books/with-authors");
        getResponse.EnsureSuccessStatusCode();

        // Assert
        var books = await getResponse.Content.ReadFromJsonAsync<List<BookResponse>>();
        Assert.Contains(books, b =>
            b.Title == "E2E Test Book" &&
            b.Authors.Any(a => a.FirstName == "AuthorFirst" && a.LastName == "AuthorLast"));
    }
}
