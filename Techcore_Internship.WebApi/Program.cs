using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Reflection;
using Techcore_Internship.Application.Services.Cache;
using Techcore_Internship.Application.Services.Entities;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.Dapper;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;
using Techcore_Internship.Data.Repositories.EF;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Data.Repositories.Mongo;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(
    AppDomain.CurrentDomain.GetAssemblies()
        .Where(assembly => assembly.FullName!.StartsWith("Techcore_Internship"))
);

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = builder.Configuration["RedisSettings:InstanceName"];
});

// Health Checks
builder.Services.AddHealthChecks();

builder.Services.AddOutputCache(options =>
    options.AddPolicy("BookPolicy", policy => 
        policy.Expire(TimeSpan.FromSeconds(60))
        .Tag("books"))
);  

// PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Techcore_Internship_Postgres_Connection")));

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

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

// Task339_7_MySettings
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

// Service registration
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// Repository registration
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBaseDapperRepository, BaseDapperRepository>();
builder.Services.AddScoped<IBookDapperRepository, BookDapperRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

var app = builder.Build();

app.UseRequestLogging(); // Task339_4_Middleware
app.UseGlobalExceptionHandler(); // Task339_5_ProblemDetails

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseOutputCache();
app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();