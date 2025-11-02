using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductReviewsController : ControllerBase
{
    private readonly IProductReviewRepository _productReviewRepository;

    public ProductReviewsController(IProductReviewRepository productReviewRepository)
    {
        _productReviewRepository = productReviewRepository;
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
        var productReview = await _productReviewRepository.GetByPredicateAsync(review => review.Id == id, cancellationToken);
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
        var productReview = await _productReviewRepository.GetListByPredicateAsync(review => true, cancellationToken);
        return Ok(productReview);
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
    public async Task<IActionResult> Create(ProductReviewEntity review, CancellationToken cancellationToken)
    {
        var productReviewId = await _productReviewRepository.CreateAsync(review, cancellationToken);
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
    public async Task<IActionResult> Update([FromRoute] Guid id, ProductReviewEntity review, CancellationToken cancellationToken)
    {
        var updatedProductReview = await _productReviewRepository.UpdateAsync(id, review, cancellationToken);
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
        var result = await _productReviewRepository.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}