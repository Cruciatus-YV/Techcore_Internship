namespace Techcore_Internship.Contracts;

public class RedisSettings
{
    public string? InstanceName { get; set; }
    public int DefaultExpirationMinutes { get; set; } = 60;
}
