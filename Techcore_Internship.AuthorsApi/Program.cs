using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Techcore_Internship.AuthorsApi.Consumers;
using Techcore_Internship.AuthorsApi.Data.Interfaces;
using Techcore_Internship.AuthorsApi.Data.Repositories;
using Techcore_Internship.AuthorsApi.Services;
using Techcore_Internship.AuthorsApi.Services.Interfaces;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Cache;
using Techcore_Internship.Data.Cache.Interfaces;
using Techcore_Internship.Data.Utils.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Techcore_Internship_Postgres_Connection")));

// Redis
builder.Services.AddCustomRedis(builder);

// Swagger
builder.Services.AddCustomSwaggerWithJwt();

// OpenTelemetry
var serviceName = "AuthorsApi";
var serviceVersion = "1.0.0";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
        }))
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(serviceName)
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Zipkin:Endpoint")
                    ?? "http://localhost:9411/api/v2/spans");
            });
    });

// MassTransit (Rabbit with retry policy)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ClearAuthorCacheConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("Cruciatus");
            h.Password("12345qwerty");
        });

        cfg.UseMessageRetry(r =>
        {
            r.Interval(3, TimeSpan.FromSeconds(5));
        });

        cfg.ReceiveEndpoint("clear-author-cache-queue", e =>
        {
            e.ConfigureConsumer<ClearAuthorCacheConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// Authentication
builder.Services.AddCustomAuthentication(builder);

// Authorization
builder.Services.AddCustomAuthorization();

// Repositories
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// Services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Techcore Internship API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();