using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IProductReviewRepository _productReviewRepository;

        public ReviewController(IProductReviewRepository productReviewRepository)
        {
            _productReviewRepository = productReviewRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var productReview = await _productReviewRepository.GetByIdAsync(id, cancellationToken);
            return Ok(productReview);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductReviewEntity review, CancellationToken cancellationToken)
        {
            var productReviewId = await _productReviewRepository.CreateAsync(review, cancellationToken);
            return Ok(productReviewId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, ProductReviewEntity review, CancellationToken cancellationToken)
        {
            var updatedProductReview = await _productReviewRepository.UpdateAsync(id, review, cancellationToken);
            return Ok(updatedProductReview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _productReviewRepository.DeleteAsync(id, cancellationToken);
            return Ok(result);
        }
    }
}
