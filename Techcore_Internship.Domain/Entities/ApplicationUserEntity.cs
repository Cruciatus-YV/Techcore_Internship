using Microsoft.AspNetCore.Identity;

namespace Techcore_Internship.Domain.Entities;

public class ApplicationUserEntity : IdentityUser
{
    public DateOnly? DateOfBirth { get; set; }
}