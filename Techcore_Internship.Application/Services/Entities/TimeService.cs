using Techcore_Internship.Application.Services.Interfaces;

namespace Techcore_Internship.Application.Services.Entities;

public class TimeService : ITimeService
{
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }

    public DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }
}
