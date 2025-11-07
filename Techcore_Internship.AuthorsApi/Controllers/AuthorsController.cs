using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.AuthorsApi.Contracts.DTOs.Requests;
using Techcore_Internship.AuthorsApi.Contracts.DTOs.Responses;
using Techcore_Internship.AuthorsApi.Services.Interfaces;

namespace Techcore_Internship.AuthorsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    /// <summary>
    /// Создание нового автора
    /// </summary>
    /// <param name="request">Данные для создания автора</param>
    /// <returns>Созданный автор</returns>
    [HttpPost]
    public async Task<ActionResult<AuthorReferenceResponse>> Create([FromBody] CreateAuthorRequest request)
    {
        var author = await _authorService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    /// <summary>
    /// Получение списка всех авторов
    /// </summary>
    /// <returns>Список авторов</returns>
    [HttpGet]
    public async Task<ActionResult<List<AuthorReferenceResponse>>> GetAll()
    {
        var authors = await _authorService.GetAllAsync();
        return Ok(authors);
    }

    /// <summary>
    /// Получение автора по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор автора</param>
    /// <returns>Данные автора</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorReferenceResponse>> GetById([FromRoute] Guid id)
    {
        var author = await _authorService.GetByIdAsync(id);
        return Ok(author);
    }

    /// <summary>
    /// Обновление информации об авторе
    /// </summary>
    /// <param name="id">Идентификатор автора</param>
    /// <param name="request">Новые данные автора</param>
    /// <returns>Обновленные данные автора</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<AuthorReferenceResponse>> Update([FromRoute] Guid id, [FromBody] UpdateAuthorInfoRequest request)
    {
        var author = await _authorService.UpdateAsync(id, request);
        return Ok(author);
    }

    /// <summary>
    /// Удаление автора
    /// </summary>
    /// <param name="id">Идентификатор автора</param>
    /// <returns>Статус операции</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var success = await _authorService.DeleteAsync(id);
        return success ? Ok() : NotFound();
    }
}