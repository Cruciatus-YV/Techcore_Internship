namespace Techcore_Internship.Contracts.DTOs.Entities.User.Requests;

public record RegisterRequest(string Email, string Password, DateOnly? DateOfBirth = null);
