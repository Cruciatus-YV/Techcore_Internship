//using Microsoft.AspNetCore.Mvc;
//using Techcore_Internship.Application.Services.Interfaces;
//using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;
//using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;

//namespace Techcore_Internship.WebApi.Controllers;

///// <summary>
///// Контроллер для управления авторами
///// Предоставляет API для выполнения операций CRUD над авторами
///// </summary>
//[Route("api/[controller]")]
//[ApiController]
//public class AuthorsController : ControllerBase
//{
//    private readonly IAuthorService _authorService;

//    /// <summary>
//    /// Конструктор контроллера авторов
//    /// </summary>
//    /// <param name="authorService">Сервис для работы с авторами</param>
//    public AuthorsController(IAuthorService authorService)
//    {
//        _authorService = authorService;
//    }

//    /// <summary>
//    /// Получить автора по идентификатору
//    /// </summary>
//    /// <param name="id">GUID идентификатор автора</param>
//    /// <param name="cancellationToken = default">Токен отмены</param>
//    /// <returns>Автор с указанным идентификатором или 404 если не найден</returns>
//    [HttpGet("{id}")]
//    public async Task<ActionResult<AuthorResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
//    {
//        var author = await _authorService.GetByIdAsync(id, cancellationToken);
//        return author == null ? NotFound() : Ok(author);
//    }

//    /// <summary>
//    /// Получить всех авторов
//    /// </summary>
//    /// <param name="includeBooks">Включать ли информацию о книгах в ответ</param>
//    /// <param name="cancellationToken = default">Токен отмены</param>
//    /// <returns>Список всех авторов</returns>
//    [HttpGet]
//    public async Task<ActionResult<List<AuthorResponse>>> GetAll([FromQuery] bool includeBooks = false, CancellationToken cancellationToken = default)
//    {
//        var authors = await _authorService.GetAllAsync(cancellationToken, includeBooks);
//        return Ok(authors);
//    }

//    /// <summary>
//    /// Создать нового автора
//    /// </summary>
//    /// <param name="request">DTO с данными для создания автора</param>
//    /// <param name="cancellationToken = default">Токен отмены</param>
//    /// <returns>Созданный автор</returns>
//    [HttpPost]
//    public async Task<IActionResult> Create([FromBody] CreateAuthorRequest request, CancellationToken cancellationToken = default)
//    {
//        var author = await _authorService.CreateAsync(request, cancellationToken);
//        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
//    }

//    /// <summary>
//    /// Обновить информацию об авторе
//    /// </summary>
//    /// <param name="id">Идентификатор обновляемого автора</param>
//    /// <param name="request">DTO с обновленными данными автора</param>
//    /// <param name="cancellationToken = default">Токен отмены</param>
//    /// <returns>200 OK при успешном обновлении, 404 Not Found если автор не найден</returns>
//    [HttpPut("{id}")]
//    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAuthorInfoRequest request, CancellationToken cancellationToken = default)
//    {
//        var result = await _authorService.UpdateAsync(id, request, cancellationToken);
//        return result ? Ok() : NotFound();
//    }

//    /// <summary>
//    /// Удалить автора по идентификатору
//    /// </summary>
//    /// <param name="id">GUID идентификатор автора для удаления</param>
//    /// <param name="cancellationToken = default">Токен отмены</param>
//    /// <returns>200 OK при успешном удалении, 404 Not Found если автор не найден</returns>
//    [HttpDelete("{id}")]
//    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
//    {
//        var result = await _authorService.DeleteAsync(id, cancellationToken);
//        return result ? Ok() : NotFound();
//    }

//    /// <summary>
//    /// Проверить существование автора по идентификатору
//    /// </summary>
//    /// <param name="id">GUID идентификатор автора</param>
//    /// <param name="cancellationToken = default">Токен отмены</param>
//    /// <returns>True если автор существует, иначе False</returns>
//    [HttpGet("{id}/exists")]
//    public async Task<ActionResult<bool>> IsExists([FromRoute] Guid id, CancellationToken cancellationToken = default)
//    {
//        var exists = await _authorService.IsExistsAsync(id, cancellationToken);
//        return Ok(exists);
//    }
//}