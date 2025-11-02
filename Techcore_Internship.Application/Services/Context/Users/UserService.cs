using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.Application.Services.Context.Users;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtService _jwtService;

    public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<IdentityResult> RegisterAsync(UserAuthRequest registerRequest)
    {
        var newUser = new IdentityUser() { UserName = registerRequest.Email, Email = registerRequest.Email };

        var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

        return result;
    }

    public async Task<(bool Success, string Token, string? Error)> LoginAsync(UserAuthRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user == null)
            return (false, "", "Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        
        if (!result.Succeeded)
            return (false, "", "Invalid email or password");

        var token = await _jwtService.GenerateTokenAsync(user);

        return (true, token, null);
    }
}
