using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Techcore_Internship.Data;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.IntegrationTests;

public class MyTestFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var authServices = services.Where(s =>
                s.ServiceType.FullName?.Contains("Authentication") == true ||
                s.ServiceType == typeof(IAuthenticationService) ||
                s.ServiceType == typeof(IAuthenticationSchemeProvider) ||
                s.ServiceType.Name?.Contains("JwtBearer") == true ||
                s.ServiceType.Name?.Contains("AuthenticationOptions") == true)
                .ToList();

            foreach (var service in authServices)
            {
                services.Remove(service);
            }

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            var dbServices = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType.Name?.Contains("DbContext") == true ||
                d.ServiceType == typeof(ApplicationDbContext))
                .ToList();

            foreach (var service in dbServices)
            {
                services.Remove(service);
            }

            // Добавляем In-Memory базу
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDB");
            }, ServiceLifetime.Scoped);
        });

        var host = base.CreateHost(builder);
        InitializeDatabase(host);
        return host;
    }

    private void InitializeDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        InitializeDbForTests(db);
    }

    private void InitializeDbForTests(ApplicationDbContext db)
    {
        db.Books.RemoveRange(db.Books);
        db.Authors.RemoveRange(db.Authors);
        db.SaveChanges();

        var author1 = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Test Author 1",
            LastName = "Test LastName 1",
            IsDeleted = false
        };

        var author2 = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Test Author 2",
            LastName = "Test LastName 2",
            IsDeleted = false
        };

        var book1 = new BookEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test Book 1",
            Year = 2020,
            IsDeleted = false,
            Authors = new List<AuthorEntity> { author1, author2 }
        };

        var book2 = new BookEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test Book 2",
            Year = 2021,
            IsDeleted = false,
            Authors = new List<AuthorEntity> { author1 }
        };

        db.Authors.AddRange(author1, author2);
        db.Books.AddRange(book1, book2);
        db.SaveChanges();
    }
}
