using Microsoft.AspNetCore.Mvc;
using Polly;
using System.Net;
using Techcore_Internship.Data.Utils.Extentions;

namespace Techcore_Internship.WebApi.Controllers
{
    [ApiController]
    [Route("api/test-polly")]
    public class TestPollyController : ControllerBase
    {
        private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

        public TestPollyController()
        {
            _pipeline = PollyExtensions.GetResiliencePipeline();
        }

        [HttpGet("load/{count:int}")]
        public async Task<IActionResult> GenerateLoad(int count = 20)
        {
            for (int i = 0; i < count; i++)
            {
                await _pipeline.ExecuteAsync(async (token) =>
                {
                    if (new Random().Next(0, 100) < 40)
                    {
                        await Task.Delay(50, token);
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }

                    await Task.Delay(20, token);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });
            }

            return Ok($"Generated {count} requests with Polly");
        }
    }
}