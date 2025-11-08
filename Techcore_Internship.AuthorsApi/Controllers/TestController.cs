using Microsoft.AspNetCore.Mvc;

namespace Techcore_Internship.AuthorsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private static int _requestCount = 0;

        [HttpGet("circuit-breaker-test")]
        public IActionResult CircuitBreakerTest()
        {
            _requestCount++;
            return StatusCode(503, $"Service unavailable - request {_requestCount}");
        }
    }
}
