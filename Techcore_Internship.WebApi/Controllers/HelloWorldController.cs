using Microsoft.AspNetCore.Mvc;

namespace Techcore_Internship.WebApi.Controllers
{
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet("hello")]
        public string Get()
        {
            return "Hello world";
        }
    }
}
