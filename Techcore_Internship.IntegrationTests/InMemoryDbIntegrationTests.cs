using System.Net;
using System.Net.Http.Json;
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
}