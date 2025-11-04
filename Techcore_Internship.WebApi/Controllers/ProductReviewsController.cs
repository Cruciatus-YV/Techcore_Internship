using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.DTOs.Entities.ProductReview.Requests;

namespace Techcore_Internship.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductReviewsController : ControllerBase
{
    private readonly IProductReviewService _productReviewService;

    public ProductReviewsController(IProductReviewService productReviewService)
    {
        _productReviewService = productReviewService;
    }

    /// <summary>
    /// Получить отзыв по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор отзыва</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Данные отзыва</returns>
    /// <response code="200">Успешное получение отзыва</response>
    /// <response code="404">Отзыв не найден</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var productReview = await _productReviewService.GetByIdAsync(id, cancellationToken);
        return Ok(productReview);
    }

    /// <summary>
    /// Получить список всех отзывов
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех отзывов</returns>
    /// <response code="200">Успешное получение списка отзывов</response>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var productReview = await _productReviewService.GetAllAsync(cancellationToken);
        return Ok(productReview);
    }

    /// <summary>
    /// Получить отзывы по идентификатору продукта
    /// </summary>
    /// <param name="productId">Идентификатор продукта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список отзывов продукта</returns>
    /// <response code="200">Успешное получение отзывов</response>
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProductId([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        var productReviews = await _productReviewService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(productReviews);
    }

    /// <summary>
    /// Получить отзывы по идентификатору пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список отзывов пользователя</returns>
    /// <response code="200">Успешное получение отзывов</response>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var productReviews = await _productReviewService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(productReviews);
    }

    /// <summary>
    /// Создать новый отзыв
    /// </summary>
    /// <param name="review">Данные отзыва</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Идентификатор созданного отзыва</returns>
    /// <response code="200">Отзыв успешно создан</response>
    /// <response code="400">Некорректные данные отзыва</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateProductReviewRequest review, CancellationToken cancellationToken)
    {
        var productReviewId = await _productReviewService.CreateAsync(review, cancellationToken);
        return Ok(productReviewId);
    }

    /// <summary>
    /// Обновить отзыв
    /// </summary>
    /// <param name="id">Идентификатор отзыва</param>
    /// <param name="review">Обновленные данные отзыва</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат обновления</returns>
    /// <response code="200">Отзыв успешно обновлен</response>
    /// <response code="404">Отзыв не найден</response>
    /// <response code="400">Некорректные данные отзыва</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, UpdateProductReviewRequest review, CancellationToken cancellationToken)
    {
        var updatedProductReview = await _productReviewService.UpdateAsync(id, review, cancellationToken);
        return Ok(updatedProductReview);
    }

    /// <summary>
    /// Удалить отзыв
    /// </summary>
    /// <param name="id">Идентификатор отзыва</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат удаления</returns>
    /// <response code="200">Отзыв успешно удален</response>
    /// <response code="404">Отзыв не найден</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _productReviewService.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}