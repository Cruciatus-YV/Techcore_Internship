using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Techcore_Internship.Data.Utils.Extentions;

var builder = WebApplication.CreateBuilder(args);

bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                         Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "True";

string configFile = isRunningInDocker ? "ocelot.docker.json" : "ocelot.local.json";

Console.WriteLine($"Using configuration: {configFile}");
Console.WriteLine($"Running in Docker: {isRunningInDocker}");

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile(configFile, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCustomAuthentication(builder);

builder.Services.AddCustomAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();
app.Run();