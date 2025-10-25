using Techcore_Internship.WebApi.Services;
using Techcore_Internship.WebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Techcore Internship API",
        Version = "v1",
        Description = "API ��� ���������� � Techcore"
    });
});

// Service registration
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();
var ignoredPathes = new HashSet<string>() { "/swagger/v1/swagger.json" }; // ��� ������� ���������� swagger � middleware
app.Use(async (context, next) => 
{
    if (!ignoredPathes.Contains(context.Request.Path))
    {
        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine($"[{startTime:HH:mm:ss}] Started {context.Request.Method} {context.Request.Path}");

        await next();

        stopwatch.Stop();
        var endTime = DateTime.UtcNow;
        Console.WriteLine($"[{endTime:HH:mm:ss}] Completed {context.Request.Method} {context.Request.Path} - {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
    }
    else 
    {
        await next();
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();