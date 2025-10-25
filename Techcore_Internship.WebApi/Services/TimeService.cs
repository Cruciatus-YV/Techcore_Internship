using Techcore_Internship.WebApi.Services.Interfaces;

namespace Techcore_Internship.WebApi.Services;

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
