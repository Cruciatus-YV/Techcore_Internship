using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Context.Users;

public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(UserManager<ApplicationUserEntity> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> AssignRoleToUserAsync(string userEmail, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null) return false;

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists) return false;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleFromUserAsync(string userEmail, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null) return false;

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }
}