namespace Techcore_Internship.Contracts;

public class RedisSettings
{
    public string InstanceName { get; set; } = "Techcore_Internship_";
    public int DefaultExpirationMinutes { get; set; } = 60;
}
