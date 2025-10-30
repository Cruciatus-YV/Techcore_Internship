﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;

namespace Techcore_Internship.WebApi.Controllers;

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

    /// <summary>
    /// Получить книгу по идентификатору (Кеширование с помощью OutputCache)
    /// </summary>
    /// <param name="id">GUID идентификатор книги</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Книга с указанным идентификатором или 404 если не найдена</returns>
    [HttpGet("{id}/output-cache")]
    [OutputCache(PolicyName = "BookPolicy")]
    public async Task<IActionResult> GetOutputCacheTest([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookService.GetByIdOutputCacheTestAsync(id, cancellationToken);
        return book == null ? NotFound() : Ok(book);
    }

    /// <summary>
    /// Получить книгу по идентификатору
    /// </summary>
    /// <param name="id">GUID идентификатор книги</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Книга с указанным идентификатором или 404 если не найдена</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookService.GetByIdAsync(id, cancellationToken);
        return book == null ? NotFound() : Ok(book);
    }

    /// <summary>
    /// Получить книги по идентификатору автора
    /// </summary>
    /// <param name="authorId">GUID идентификатор автора</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Список книг указанного автора</returns>
    [HttpGet("by-author/{authorId}")]
    public async Task<IActionResult> GetByAuthorId([FromRoute] Guid authorId, CancellationToken cancellationToken = default)
    {
        var books = await _bookService.GetByAuthorIdAsync(authorId, cancellationToken);
        return Ok(books ?? new List<BookResponse>());
    }

    /// <summary>
    /// Получить все книги с авторами
    /// </summary>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Список всех книг с информацией об авторах</returns>
    [HttpGet("with-authors")]
    public async Task<IActionResult> GetAllWithAuthors(CancellationToken cancellationToken = default)
    {
        var books = await _bookService.GetAllWithAuthorsAsync(cancellationToken);
        return Ok(books ?? new List<BookResponse>());
    }

    /// <summary>
    /// Получить все книги с авторами (Dapper)
    /// </summary>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Список всех книг с информацией об авторах</returns>
    [HttpGet("with-authors-dapper")]
    public async Task<IActionResult> GetAllWithAuthorsFromDapper(CancellationToken cancellationToken = default)
    {
        var books = await _bookService.GetAllWithAuthorsFromDapperAsync(cancellationToken);
        return Ok(books ?? new List<BookResponse>());
    }

    /// <summary>
    /// Получить книги по году издания
    /// </summary>
    /// <param name="year">Год издания</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Список книг указанного года издания</returns>
    [HttpGet("by-year/{year}")]
    public async Task<IActionResult> GetByYear([FromRoute] int year, CancellationToken cancellationToken = default)
    {
        var books = await _bookService.GetByYearAsync(year, cancellationToken);
        return Ok(books ?? new List<BookResponse>());
    }

    /// <summary>
    /// Создать новую книгу с существующими авторами
    /// </summary>
    /// <param name="request">DTO с данными для создания книги</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Созданная книга</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        var newBook = await _bookService.CreateAsync(request, cancellationToken);
        return Ok(newBook);
    }

    /// <summary>
    /// Создать новую книгу с новыми и/или существующими авторами
    /// </summary>
    /// <param name="request">DTO с данными для создания книги и авторов</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>Идентификатор созданной книги</returns>
    [HttpPost("with-authors")]
    public async Task<IActionResult> CreateWithAuthors([FromBody] CreateBookWithAuthorsRequest request, CancellationToken cancellationToken = default)
    {
        var bookId = await _bookService.CreateWithAuthorsAsync(request, cancellationToken);
        return Ok(bookId);
    }

    /// <summary>
    /// Полностью обновить существующую книгу
    /// </summary>
    /// <param name="id">Идентификатор обновляемой книги</param>
    /// <param name="request">DTO с обновленными данными книги</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>200 OK при успешном обновлении, 404 Not Found если книга не найдена</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        return await _bookService.UpdateAsync(id, request, cancellationToken)
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Обновить авторов книги
    /// </summary>
    /// <param name="id">Идентификатор книги</param>
    /// <param name="request">DTO с новыми и/или существующими авторами</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>200 OK при успешном обновлении, 404 Not Found если книга не найдена</returns>
    [HttpPut("{id}/authors")]
    public async Task<IActionResult> UpdateAuthors([FromRoute] Guid id, [FromBody] UpdateBookAuthorsRequest request, CancellationToken cancellationToken = default)
    {
        return await _bookService.UpdateAuthorsAsync(id, request, cancellationToken)
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Обновить информацию о книге (название и год)
    /// </summary>
    /// <param name="id">Идентификатор книги</param>
    /// <param name="request">DTO с обновленной информацией о книге</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>200 OK при успешном обновлении, 404 Not Found если книга не найдена</returns>
    [HttpPut("{id}/info")]
    public async Task<IActionResult> UpdateBookInfo([FromRoute] Guid id, [FromBody] UpdateBookInfoRequest request, CancellationToken cancellationToken = default)
    {
        return await _bookService.UpdateBookInfoAsync(id, request, cancellationToken)
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Обновить название книги по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор обновляемой книги</param>
    /// <param name="title">Новое название книги</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>200 OK при успешном обновлении, 404 Not Found если книга не найдена</returns>
    [HttpPatch("{id}/title")]
    public async Task<IActionResult> UpdateTitle([FromRoute] Guid id, [FromBody] string title, CancellationToken cancellationToken = default)
    {
        return await _bookService.UpdateTitleAsync(id, title, cancellationToken)
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Обновить год издания книги
    /// </summary>
    /// <param name="id">Идентификатор книги</param>
    /// <param name="year">Новый год издания</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>200 OK при успешном обновлении, 404 Not Found если книга не найдена</returns>
    [HttpPatch("{id}/year")]
    public async Task<IActionResult> UpdateYear([FromRoute] Guid id, [FromBody] int year, CancellationToken cancellationToken = default)
    {
        return await _bookService.UpdateYearAsync(id, year, cancellationToken)
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Удалить книгу по идентификатору
    /// </summary>
    /// <param name="id">GUID идентификатор книги для удаления</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>200 OK при успешном удалении, 404 Not Found если книга не найдена</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return await _bookService.DeleteAsync(id, cancellationToken)
            ? Ok()
            : NotFound();
    }

    /// <summary>
    /// Проверить существование книги по идентификатору
    /// </summary>
    /// <param name="id">GUID идентификатор книги</param>
    /// <param name="cancellationToken = default">Токен отмены</param>
    /// <returns>True если книга существует, иначе False</returns>
    [HttpGet("{id}/exists")]
    public async Task<IActionResult> Exists([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _bookService.Exists(id, cancellationToken);
        return Ok(exists);
    }
}