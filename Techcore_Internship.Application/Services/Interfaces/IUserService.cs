using Microsoft.AspNetCore.Identity;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.Application.Services.Interfaces;

public interface IUserService
{
    Task<IdentityResult> RegisterAsync(UserAuthRequest registerRequest);
}
