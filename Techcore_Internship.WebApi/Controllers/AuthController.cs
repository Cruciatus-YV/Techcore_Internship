using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] UserAuthRequest registerRequest)
    {
        var result = await _userService.RegisterAsync(registerRequest);

        if (result.Succeeded)
        {
            return Ok(new
            {
                success = true,
                message = "User registered successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors.Select(e => e.Description)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] UserAuthRequest loginRequest)
    {
        var (success, token, error) = await _userService.LoginAsync(loginRequest);

        if (success)
        {
            return Ok(new { token });
        }

        return Unauthorized(new { error });
    }
}