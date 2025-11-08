using System.Text;
using System.Text.Json;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

namespace Techcore_Internship.Application.Services.Context.Authors;

public class AuthorHttpService : IAuthorHttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    // HttpClient теперь инжектируется через конструктор!
    public AuthorHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/authors/{id}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<AuthorResponse>(content, _jsonOptions);
        }
        catch (Exception)
        {
            throw new Exception($"Error getting author {id}");
        }
    }

    public async Task<List<AuthorResponse>?> GetByIdsAsync(List<Guid> requestedIds, CancellationToken cancellationToken = default)
    {
        var authors = new List<AuthorResponse>();

        try
        {
            foreach (var authorId in requestedIds)
            {
                var author = await GetByIdAsync(authorId, cancellationToken);
                if (author != null) authors.Add(author);
            }
            return authors;
        }
        catch (Exception)
        {
            throw new Exception("Error getting authors batch");
        }
    }

    public async Task<List<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default, bool includeBooks = false)
    {
        try
        {
            var url = includeBooks ? "/api/authors?includeBooks=true" : "/api/authors";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<List<AuthorResponse>>(content, _jsonOptions) ?? new List<AuthorResponse>();
        }
        catch (Exception)
        {
            throw new Exception("Error getting all authors");
        }
    }

    public async Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/authors", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<AuthorResponse>(responseContent, _jsonOptions)!;
        }
        catch (Exception)
        {
            throw new Exception("Error creating author");
        }
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateAuthorInfoRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/authors/{id}", content, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception)
        {
            throw new Exception($"Error updating author {id}");
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/authors/{id}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception)
        {
            throw new Exception($"Error deleting author {id}");
        }
    }

    public async Task<bool> IsExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/authors/{id}/exists", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception)
        {
            throw new Exception($"Error checking if author exists {id}");
        }
    }
}