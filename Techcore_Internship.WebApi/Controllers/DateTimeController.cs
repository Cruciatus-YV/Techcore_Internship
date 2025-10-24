using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.WebApi.Services.Interfaces;

namespace Techcore_Internship.WebApi.Controllers
{
    [ApiController]
    public class DateTimeController : ControllerBase
    {
        private readonly ITimeService _timeService;
        public DateTimeController(ITimeService timeService)
        {
            _timeService = timeService;
        }

        [HttpGet("time")]
        public string GetCurrentTime()
        {
            return _timeService.GetCurrentTime();
        }
        [HttpGet("datetime")]

        public DateTime GetCurrentDateTime()
        {
            return _timeService.GetCurrentDateTime();
        }
    }
}
