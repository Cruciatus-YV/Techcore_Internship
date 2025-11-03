using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateTokenAsync(ApplicationUserEntity user, IList<string> roles);
}
