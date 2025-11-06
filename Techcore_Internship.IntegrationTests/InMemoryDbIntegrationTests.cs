using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;
using Techcore_Internship.IntegrationTests;
using Xunit;

public class InMemoryDbIntegrationTests
{
    [Fact]
    public async Task GetBooks_WithInMemoryDb_ShouldReturnOk()
    {
        // Arrange
        var webApplicationFactory = new MyTestFactory();
        var httpClient = webApplicationFactory.CreateClient();

        // Act
        var response = await httpClient.GetAsync("api/Books/with-authors");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_WithInMemoryDb_ShouldReturnOk()
    {
        // Arrange
        var webApplicationFactory = new MyTestFactory();
        var httpClient = webApplicationFactory.CreateClient();

        // Act
        var response = await httpClient.GetAsync("api/Authors");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var webApplicationFactory = new MyTestFactory();
        var httpClient = webApplicationFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

        var badDto = new
        {
            Title = "",
            Year = 1200,
            Authors = new List<Guid>()
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/Books", badDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var webApplicationFactory = new MyTestFactory();
        var httpClient = webApplicationFactory.CreateClient();

        var validDto = new CreateBookWithAuthorsRequest(
            "Valid Book Title",
            2023,
            new List<CreateAuthorRequest> { new CreateAuthorRequest("Author First", "Author Last") }
        );

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/Books/with-authors", validDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithAuth_ShouldReturnOk()
    {
        // Arrange
        var webApplicationFactory = new MyTestFactory();
        var httpClient = webApplicationFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

        var validDto = new CreateBookWithAuthorsRequest(
            "Valid Book Title",
            2023,
            new List<CreateAuthorRequest> { new CreateAuthorRequest("Author First", "Author Last") }
        );

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/Books/with-authors", validDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}