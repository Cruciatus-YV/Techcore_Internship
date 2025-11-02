using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.Application.Services.Entities;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterAsync(UserAuthRequest registerRequest)
    {
        var newUser = new IdentityUser() { UserName = registerRequest.Email, Email = registerRequest.Email };

        var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

        return result;
    }

    public async Task<SignInResult> LoginAsync(UserAuthRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user == null)
        {
            return SignInResult.Failed;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: false);

        return result;
    }
}
