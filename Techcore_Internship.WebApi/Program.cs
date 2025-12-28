using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Polly;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Techcore_Internship.Application.Services;
using Techcore_Internship.Application.Services.Background;
using Techcore_Internship.Application.Services.Context;
using Techcore_Internship.Application.Services.Context.Authors;
using Techcore_Internship.Application.Services.Context.Books;
using Techcore_Internship.Application.Services.Context.ProductReviews;
using Techcore_Internship.Application.Services.Context.Users;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts.Configurations;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Authorization.Handlers;
using Techcore_Internship.Data.Cache;
using Techcore_Internship.Data.Cache.Interfaces;
using Techcore_Internship.Data.Repositories.Dapper;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;
using Techcore_Internship.Data.Repositories.EF;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Data.Repositories.Mongo;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Data.Utils.Extentions;
using Techcore_Internship.Domain.Entities;
using Techcore_Internship.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

string configPath = "/app/config";

if (Directory.Exists(configPath))
{
    foreach (var file in Directory.GetFiles(configPath))
    {
        var key = Path.GetFileName(file);
        var value = File.ReadAllText(file).Trim();

        builder.Configuration.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>(key.Replace("__", ":"), value)
        });

        Console.WriteLine($"Loaded config from file: {key} = {value}");
    }
}
else
{
    Console.WriteLine($"Config path not found: {configPath}. Running with default configuration.");
}

// Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", context.HostingEnvironment.ApplicationName)
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(
            uri: context.Configuration["Loki:Address"] ?? "http://localhost:3100",
            labels: new[]
            {
                new LokiLabel { Key = "app", Value = context.HostingEnvironment.ApplicationName },
                new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName }
            });
});

// Basic services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Techcore_Internship_Postgres_Connection")
    )
);

// MongoDB
builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

// Identity
builder.Services.AddIdentity<ApplicationUserEntity, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authentication / Authorization
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
builder.Services.AddCustomAuthentication(builder);
builder.Services.AddCustomAuthorization();

// Caching
builder.Services.AddCustomRedis(builder);
builder.Services.AddOutputCache();

// Messaging
builder.Services.AddCustomMassTransit();
builder.Services.AddKafka();

// OpenTelemetry
builder.Services.AddCustomOpenTelemetry(builder.Configuration);

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(
    AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName!.StartsWith("Techcore_Internship"))
);

// Health checks
builder.Services.AddHealthChecks();

// Swagger
builder.Services.AddCustomSwaggerWithJwt();

// App settings
builder.Services.Configure<MySettings>(
    builder.Configuration.GetSection("MySettings")
);
builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection("RedisSettings")
);

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBaseDapperRepository, BaseDapperRepository>();
builder.Services.AddScoped<IBookDapperRepository, BookDapperRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IBookViewAnalyticsRepository, BookViewAnalyticsRepository>();

// Services
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorHttpService, AuthorHttpService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<OpenApiInformationService>();

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

// Background services
builder.Services.AddHostedService<AverageRatingCalculatorService>();
builder.Services.AddHostedService<KafkaConsumerService>();

// HttpClient + Polly
builder.Services
    .AddHttpClient<IAuthorHttpService, AuthorHttpService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["AuthorsApi:BaseUrl"]);
    })
    .AddResilienceHandler(
        "AuthorsApiPipeline",
        (pipelineBuilder, _) =>
        {
            pipelineBuilder.AddPipeline(
                PollyExtensions.GetResiliencePipeline()
            );
        });

var app = builder.Build();

// Middleware
app.UseRequestLogging();
app.UseGlobalExceptionHandler();

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Techcore Internship API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");
app.MapHealthChecks("/healthz");

// Database initialization
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager);

    Console.WriteLine("Database initialized successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization failed: {ex}");
}

app.Run();

// Helpers
static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "User", "Admin", "SuperAdmin" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

public partial class Program { }
