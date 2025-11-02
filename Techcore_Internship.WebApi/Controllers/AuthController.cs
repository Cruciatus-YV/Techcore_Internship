using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserRegisterRequest user)
    {
        var result = await _userService.RegisterAsync(user);

        return Ok(result);
    }
}
