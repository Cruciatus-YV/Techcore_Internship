using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using System.Text;
using Techcore_Internship.Application.Authorization.Handlers;
using Techcore_Internship.Application.Authorization.Policies;
using Techcore_Internship.Application.Authorization.Reqirements;
using Techcore_Internship.Application.Services;
using Techcore_Internship.Application.Services.Background;
using Techcore_Internship.Application.Services.Cache;
using Techcore_Internship.Application.Services.Context.Authors;
using Techcore_Internship.Application.Services.Context.Books;
using Techcore_Internship.Application.Services.Context.ProductReview;
using Techcore_Internship.Application.Services.Context.Users;
using Techcore_Internship.Application.Services.Entities;
using Techcore_Internship.Application.Services.Interfaces;
using Techcore_Internship.Contracts;
using Techcore_Internship.Contracts.Configurations;
using Techcore_Internship.Data;
using Techcore_Internship.Data.Repositories.Dapper;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;
using Techcore_Internship.Data.Repositories.EF;
using Techcore_Internship.Data.Repositories.EF.Interfaces;
using Techcore_Internship.Data.Repositories.Mongo;
using Techcore_Internship.Data.Repositories.Mongo.Interfaces;
using Techcore_Internship.Domain.Entities;
using Techcore_Internship.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Basic Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Techcore_Internship_Postgres_Connection")));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

// Identity
builder.Services.AddIdentity<ApplicationUserEntity, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AgePolicies.OLDER_THEN_18, policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));

    options.AddPolicy(AgePolicies.OLDER_THEN_21, policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(21)));
});


// Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = builder.Configuration["RedisSettings:InstanceName"];
});
builder.Services.AddOutputCache(options =>
    options.AddPolicy("BookPolicy", policy => policy.Expire(TimeSpan.FromSeconds(60)).Tag("books")));

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(
    AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName!.StartsWith("Techcore_Internship"))
);

// Health Checks
builder.Services.AddHealthChecks();

// Swagger with JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Techcore Internship API",
        Version = "v1",
        Description = "API для стажировки в Techcore"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен в формате: Bearer {токен}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
         {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 },
                 Scheme = "oauth2",
                 Name = "Bearer",
                 In = ParameterLocation.Header,
             },
             new List<string>()
        }
    });
});

// App settings
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBaseDapperRepository, BaseDapperRepository>();
builder.Services.AddScoped<IBookDapperRepository, BookDapperRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

// Services
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
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

// HttpClientFactory
builder.Services.AddHttpClient();

var app = builder.Build();

// Middleware pipeline
app.UseRequestLogging();
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/healthz");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRoles(roleManager);
}

app.Run();

async Task SeedRoles(RoleManager<IdentityRole> roleManager)
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