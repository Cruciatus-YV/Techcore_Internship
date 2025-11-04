namespace Techcore_Internship.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AssignRoleToUserAsync(string userEmail, string roleName);
        Task<bool> RemoveRoleFromUserAsync(string userEmail, string roleName);
    }
}