using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsHttpController : ControllerBase
{
    private readonly IAuthorHttpService _authorHttpService;

    public AuthorsHttpController(IAuthorHttpService authorHttpService)
    {
        _authorHttpService = authorHttpService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        //var authors = await _authorHttpService.GetAllAsync(cancellationToken)
        var authors = new List<AuthorResponse>();
        return Ok(authors);
    }
}