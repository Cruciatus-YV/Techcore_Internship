using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

namespace Techcore_Internship.Application.Services.Context.Authors;

public class AuthorHttpService : IAuthorHttpService
{
    private readonly HttpClient _httpClient;

    public AuthorHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuthorService");
    }

    /*
    public async Task<List<AuthorDto>> GetAllAuthorsAsync()
    {
        var response = await _httpClient.GetAsync("/api/authors");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
    }
    */
}
