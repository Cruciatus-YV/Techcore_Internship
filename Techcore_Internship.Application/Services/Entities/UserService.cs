using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.Application.Services.Entities;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> RegisterAsync(UserAuthRequest registerRequest)
    {
        var newUser = new IdentityUser() { UserName = registerRequest.Email, Email = registerRequest.Email };

        var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

        return result;
    }
}
