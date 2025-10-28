using Microsoft.AspNetCore.Mvc;

namespace Techcore_Internship.WebApi.Controllers;

// Task339_1_HelloWorldApi
[ApiController]
public class HelloWorldController : ControllerBase
{
    [HttpGet("hello")]
    public string Get()
    {
        return "Hello world";
    }
}
 