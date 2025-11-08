using Microsoft.AspNetCore.Mvc;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Task349_4_PollyController : ControllerBase
{
    private static int _requestCount = 0;

    [HttpGet("unstable")]
    public IActionResult GetUnstable()
    {
        _requestCount++;

        // Первые 2 запроса возвращают 503, третий - 200
        if (_requestCount <= 2)
        {
            return StatusCode(503, $"Service unavailable - attempt {_requestCount}");
        }

        return Ok($"Success after {_requestCount} attempts");
    }
}
