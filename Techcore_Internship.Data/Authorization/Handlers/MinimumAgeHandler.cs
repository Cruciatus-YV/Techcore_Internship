using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Techcore_Internship.Data.Authorization.Reqirements;

namespace Techcore_Internship.Data.Authorization.Handlers;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);

        if (dateOfBirthClaim == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Date of birth claim not found"));
            return Task.CompletedTask;
        }

        if (!DateTime.TryParse(dateOfBirthClaim.Value, out DateTime dateOfBirth))
        {
            context.Fail(new AuthorizationFailureReason(this, "Invalid date of birth format"));
            return Task.CompletedTask;
        }

        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        if (age >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, $"User must be at least {requirement.MinimumAge} years old. Current age: {age}"));
        }

        return Task.CompletedTask;
    }
}
