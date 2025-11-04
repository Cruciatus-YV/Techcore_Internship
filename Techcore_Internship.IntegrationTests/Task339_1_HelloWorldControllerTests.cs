using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace Techcore_Internship.IntegrationTests;

public class Task339_1_HelloWorldControllerTests
{
    [Fact]
    public async Task GetHello_SendRequest_ShouldReturnOk()
    {
        // Arrange

        var webApplicationFactory = new WebApplicationFactory<Program>();

        var httpClient = webApplicationFactory.CreateClient();

        // Act

        var response = await httpClient.GetAsync("api/Task339_1_HelloWorld/hello");

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
