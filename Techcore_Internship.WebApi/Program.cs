using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Reflection;
using Techcore_Internship.Application.Services;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Data;
using Techcore_Internship.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Task339_6_FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(
    AppDomain.CurrentDomain.GetAssemblies()
        .Where(assembly => assembly.FullName!.StartsWith("Techcore_Internship"))
);

// Task339_10_Health Checks
builder.Services.AddHealthChecks();

// Task341_1_EntityFrameworkCore_PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Techcore_Internship_DB_Connection")));

try
{
    using var connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("Techcore_Internship_DB_Connection"));
    connection.Open();
    Console.WriteLine("Database connection successful!");
    connection.Close();
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Techcore Internship API",
        Version = "v1",
        Description = "API для стажировки в Techcore"
    });

    // Task339_9_SwaggerXmlComments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Task339_7_MySettings
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));

// Service registration
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Task339_4_Middleware
var ignoredPathes = new HashSet<string>() { "/swagger/v1/swagger.json" }; // Для примера игнорируем swagger в middleware
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

// Task339_5_ProblemDetails
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = errorFeature?.Error;

        var problemDetails = new ProblemDetails
        {
            Type = exception?.GetType()?.Name,
            Title = "Internal Server Error",
            Status = 500,
            Detail = exception?.Message,
            Instance = context.Request.Path,
        };

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
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

app.MapHealthChecks("/healthz");

app.Run();