using System.Text.Json;
using Techcore_Internship.Data.Utils.Extentions;
using Techcore_Internship.Gateway.Aggregators;

var builder = WebApplication.CreateBuilder(args);

bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                         Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "True";

string configFile = isRunningInDocker ? "yarp.docker.json" : "yarp.local.json";

Console.WriteLine($"Using configuration: {configFile}");
Console.WriteLine($"Running in Docker: {isRunningInDocker}");

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile(configFile, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddHttpClient();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddSingleton<BookDetailsAggregator>();

builder.Services.AddCustomAuthentication(builder);
builder.Services.AddCustomAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/details/{id}", async (HttpContext context, BookDetailsAggregator aggregator, string id) =>
{
    var result = await aggregator.AggregateBookDetailsAsync(id);

    if (result == null)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Book not found");
        return;
    }

    var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    });

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(json);
});

app.MapReverseProxy();

app.Run();