using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Authors API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});

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
            .AddSource("MassTransit")
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
            })
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Zipkin:Endpoint")
                    ?? "http://zipkin:9411/api/v2/spans");
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter(serviceName)
            .AddMeter("MassTransit")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });
builder.Services.AddCustomOpenTelemetry2();

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
            h.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
            h.Heartbeat(TimeSpan.FromSeconds(10));
        });

        cfg.UseMessageRetry(r =>
        {
            r.Exponential(10,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(2));
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
app.UseOpenTelemetryPrometheusScrapingEndpoint();
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