using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.Application.Services.Interfaces;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
public class Task339_2_DateTimeController : ControllerBase
{
    private readonly ITimeService _timeService;
    public Task339_2_DateTimeController(ITimeService timeService)
    {
        _timeService = timeService;
    }

    /// <summary>
    /// Получить текущее время 
    /// </summary>
    /// <returns></returns>
    [HttpGet("time")]
    public string GetCurrentTime()
    {
        return _timeService.GetCurrentTime();
    }

    /// <summary>
    /// Получить текущую дату и время
    /// </summary>
    /// <returns></returns>
    [HttpGet("datetime")]
    public DateTime GetCurrentDateTime()
    {
        return _timeService.GetCurrentDateTime();
    }
}
