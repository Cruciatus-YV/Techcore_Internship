using Microsoft.AspNetCore.Mvc;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
public class Task339_1_HelloWorldController : ControllerBase
{
    /// <summary>
    /// Вывести строку "Hello world"
    /// </summary>
    /// <returns></returns>
    [HttpGet("hello")]
    public string Get()
    {
        return "Hello world";
    }
}
 