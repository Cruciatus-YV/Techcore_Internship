using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services;

namespace Techcore_Internship.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenApiInformationController : ControllerBase
    {
        private readonly OpenApiInformationService _openApiInformationService;

        public OpenApiInformationController(OpenApiInformationService openApiInformationService)
        {
            _openApiInformationService = openApiInformationService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var priceData = await _openApiInformationService.GetOpenApiInformation();

            return Ok(priceData);
        }
    }
}
