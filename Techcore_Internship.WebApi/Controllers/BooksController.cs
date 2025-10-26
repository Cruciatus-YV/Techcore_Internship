using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;

namespace Techcore_Internship.WebApi.Controllers;

// Task339_3_BookApi

/// <summary>
/// Контроллер для управления книгами
/// Предоставляет API для выполнения операций CRUD над книгами
/// </summary>
[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly MySettings _mySettings;

    /// <summary>
    /// Конструктор контроллера книг
    /// </summary>
    /// <param name="bookService">Сервис для работы с книгами</param>
    /// <param name="mySettings">Настройки приложения</param>
    public BooksController(IBookService bookService, IOptions<MySettings> mySettings)
    {
        _mySettings = mySettings.Value;
        _bookService = bookService;
    }

    /// <summary>
    /// Получить текущие настройки приложения
    /// </summary>
    /// <returns>Объект с настройками приложения</returns>
    [HttpGet("settings")]
    public IActionResult Settings()
    {
        return Ok(_mySettings);
    }

    /// <summary>
    /// Выбросить исключение NotImplementedException
    /// </summary>
    /// <returns>Исключение NotImplementedException</returns>
    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new NotImplementedException();
    }

    // Task339_8_AsyncContollers

    /// <summary>
    /// Получить все книги
    /// </summary>
    /// <returns>Список всех книг</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _bookService.GetAll());
    }

    /// <summary>
    /// Получить книгу по идентификатору
    /// </summary>
    /// <param name="id">GUID идентификатор книги</param>
    /// <returns>Книга с указанным идентификатором или 404 если не найдена</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var book = await _bookService.Get(id);

        return book == null
            ? NotFound()
            : Ok(book);
    }

    /// <summary>
    /// Создать новую книгу
    /// </summary>
    /// <param name="book">DTO с данными для создания книги</param>
    /// <returns>Созданная книга</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequestDto book)
    {
        var newBook = await _bookService.Create(book);

        return Ok(newBook);
    }

    [HttpPost("with-author")]
    public async Task<IActionResult> CreateWithAuthor([FromBody] CreateBookWithAuthorRequestDto request)
    {
        var result = await _bookService.CreateBookWithAuthor(request);

        return Ok(result);
    }

    /// <summary>
    /// Обновить существующую книгу
    /// </summary>
    /// <param name="request">DTO с обновленными данными книги</param>
    /// <returns>200 OK при успешном обновлении, 400 Bad Request при ошибке</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BookResponseDto request)
    {
        return await _bookService.Update(request)
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Обновить название книги по идентификатору
    /// </summary>
    /// <param name="id">GUID идентификатор книги</param>
    /// <param name="request">Новое название книги</param>
    /// <returns>200 OK при успешном обновлении, 400 Bad Request при ошибке</returns>
    [HttpPatch("{id}/update-title")]
    public async Task<IActionResult> UpdateTitle([FromRoute] Guid id, [FromBody] string request)
    {
        return await _bookService.UpdateTitle(id, request)
            ? Ok()
            : BadRequest();
    }

    /// <summary>
    /// Удалить книгу по идентификатору
    /// </summary>
    /// <param name="id">GUID идентификатор книги для удаления</param>
    /// <returns>200 OK при успешном удалении, 400 Bad Request при ошибке</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return await _bookService.Delete(id)
            ? Ok()
            : BadRequest();
    }
}