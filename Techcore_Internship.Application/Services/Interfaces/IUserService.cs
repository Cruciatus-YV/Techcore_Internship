using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IUserService
{
    Task<(bool Success, string Token, string? Error)> LoginAsync(UserAuthRequest loginRequest);
    Task<IdentityResult> RegisterAsync(UserAuthRequest registerRequest);
}
