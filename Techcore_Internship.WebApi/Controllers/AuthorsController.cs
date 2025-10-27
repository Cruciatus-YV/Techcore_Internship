using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;

namespace Techcore_Internship.WebApi.Controllers;

/// <summary>
/// Контроллер для управления авторами
/// Предоставляет API для выполнения операций CRUD над авторами
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    /// <summary>
    /// Конструктор контроллера авторов
    /// </summary>
    /// <param name="authorService">Сервис для работы с авторами</param>
    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    ///// <summary>
    ///// Получить всех авторов
    ///// </summary>
    ///// <returns>Список всех авторов</returns>
    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    return Ok(await _authorService.GetAll());
    //}

    ///// <summary>
    ///// Получить автора по идентификатору
    ///// </summary>
    ///// <param name="id">GUID идентификатор автора</param>
    ///// <returns>Автор с указанным идентификатором или 404 если не найден</returns>
    //[HttpGet("{id}")]
    //public async Task<IActionResult> Get([FromRoute] Guid id)
    //{
    //    var author = await _authorService.Get(id);

    //    return author == null
    //        ? NotFound()
    //        : Ok(author);
    //}

    ///// <summary>
    ///// Создать нового автора
    ///// </summary>
    ///// <param name="author">DTO с данными для создания автора</param>
    ///// <returns>Созданный автор</returns>
    //[HttpPost]
    //public async Task<IActionResult> Create([FromBody] CreateAuthorRequestDto author)
    //{
    //    var newAuthor = await _authorService.Create(author);

    //    return Ok(newAuthor);
    //}

    ///// <summary>
    ///// Обновить существующего автора
    ///// </summary>
    ///// <param name="request">DTO с обновленными данными автора</param>
    ///// <returns>200 OK при успешном обновлении, 400 Bad Request при ошибке</returns>
    //[HttpPut]
    //public async Task<IActionResult> Update([FromBody] AuthorResponseDto request)
    //{
    //    return await _authorService.Update(request)
    //        ? Ok()
    //        : BadRequest();
    //}

    ///// <summary>
    ///// Обновить имя автора по идентификатору
    ///// </summary>
    ///// <param name="id">GUID идентификатор автора</param>
    ///// <param name="firstName">Новое имя автора</param>
    ///// <param name="lastName">Новая фамилия автора</param>
    ///// <returns>200 OK при успешном обновлении, 400 Bad Request при ошибке</returns>
    //[HttpPatch("{id}/update-name")]
    //public async Task<IActionResult> UpdateName([FromRoute] Guid id, [FromBody] string request)
    //{
    //    return await _authorService.UpdateName(id, request)
    //        ? Ok()
    //        : BadRequest();
    //}

    ///// <summary>
    ///// Удалить автора по идентификатору
    ///// </summary>
    ///// <param name="id">GUID идентификатор автора для удаления</param>
    ///// <returns>200 OK при успешном удалении, 400 Bad Request при ошибке</returns>
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete([FromRoute] Guid id)
    //{
    //    return await _authorService.Delete(id)
    //        ? Ok()
    //        : BadRequest();
    //}

    ///// <summary>
    ///// Получить авторов по имени
    ///// </summary>
    ///// <param name="firstName">Имя автора</param>
    ///// <returns>Список авторов с указанным именем</returns>
    //[HttpGet("by-first-name/{firstName}")]
    //public async Task<IActionResult> GetAuthorsByFirstName([FromRoute] string firstName)
    //{
    //    return Ok(await _authorService.GetAuthorsByFirstName(firstName));
    //}

    ///// <summary>
    ///// Получить авторов по фамилии
    ///// </summary>
    ///// <param name="lastName">Фамилия автора</param>
    ///// <returns>Список авторов с указанной фамилией</returns>
    //[HttpGet("by-last-name/{lastName}")]
    //public async Task<IActionResult> GetAuthorsByLastName([FromRoute] string lastName)
    //{
    //    return Ok(await _authorService.GetAuthorsByLastName(lastName));
    //}
}