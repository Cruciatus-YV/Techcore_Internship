using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Task349_5_PollyCircuitBreakerController : ControllerBase
{
    private readonly IAuthorHttpService _authorHttpService;

    public Task349_5_PollyCircuitBreakerController(IAuthorHttpService authorHttpService)
    {
        _authorHttpService = authorHttpService;
    }

    [HttpGet("test-via-author-service")]
    public async Task<ActionResult> TestViaAuthorService()
    {
        var results = new List<dynamic>();

        for (int i = 1; i <= 6; i++)
        {
            try
            {
                var result = await _authorHttpService.TestCircuitBreakerAsync();
                results.Add(JsonSerializer.Deserialize<AuthorResponse>(result));
            }
            catch (Exception ex)
            {
                results.Add($"Request {i}: {ex.GetType().Name} - {ex.Message}");
            }

            await Task.Delay(100);
        }

        return Ok(results);
    }
}
