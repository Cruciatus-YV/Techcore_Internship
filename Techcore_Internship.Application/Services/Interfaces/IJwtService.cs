using Microsoft.AspNetCore.Identity;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(IdentityUser user);
}
