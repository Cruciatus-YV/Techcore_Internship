using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Context.Users;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly IJwtService _jwtService;

    public UserService(
        UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterRequest registerRequest)
    {
        var newUser = new ApplicationUserEntity()
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            DateOfBirth = registerRequest.DateOfBirth
        };

        var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, "User");
        }

        return result;
    }

    public async Task<(bool Success, string Token, string? Error)> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user == null)
            return (false, "", "Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

        if (!result.Succeeded)
            return (false, "", "Invalid email or password");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateTokenAsync(user, roles);

        return (true, token, null);
    }
}