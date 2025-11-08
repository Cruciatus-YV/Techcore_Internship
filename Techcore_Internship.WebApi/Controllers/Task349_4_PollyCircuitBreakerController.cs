using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Task349_4_PollyCircuitBreakerController : ControllerBase
{
    private readonly IAuthorHttpService _authorHttpService;

    public Task349_4_PollyCircuitBreakerController(IAuthorHttpService authorHttpService)
    {
        _authorHttpService = authorHttpService;
    }

    [HttpGet("test-via-author-service")]
    public async Task<ActionResult> TestViaAuthorService()
    {
        var results = new List<string>();

        for (int i = 1; i <= 6; i++)
        {
            try
            {
                // AuthorHttpService будет ходить в AuthorService /api/test/circuit-breaker-test
                var result = await _authorHttpService.TestCircuitBreakerAsync();
                results.Add($"Request {i}: Success");
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
