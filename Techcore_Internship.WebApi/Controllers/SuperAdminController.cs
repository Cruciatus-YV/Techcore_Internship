using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "SuperAdmin")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] RoleRequest request)
    {
        var result = await _roleService.AssignRoleToUserAsync(request.UserEmail, request.RoleName);

        if (result)
            return Ok($"Role {request.RoleName} assigned to {request.UserEmail}");
        else
            return BadRequest("Failed to assign role");
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole([FromBody] RoleRequest request)
    {
        var result = await _roleService.RemoveRoleFromUserAsync(request.UserEmail, request.RoleName);

        if (result)
            return Ok($"Role {request.RoleName} removed from {request.UserEmail}");
        else
            return BadRequest("Failed to remove role");
    }
}