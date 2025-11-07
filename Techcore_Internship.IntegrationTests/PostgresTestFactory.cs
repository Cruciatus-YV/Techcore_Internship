using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Techcore_Internship.Application.Services.Background;
using Techcore_Internship.Data;
using Techcore_Internship.Domain.Entities;
using Techcore_Internship.IntegrationTests;
using Testcontainers.PostgreSql;

public class PostgresTestFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgresContainer;

    public PostgresTestFactory()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:16")
            .Build();

        _postgresContainer.StartAsync().GetAwaiter().GetResult();
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var hostedServices = services
                .Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType) ||
                            s.ImplementationType == typeof(AverageRatingCalculatorService))
                .ToList();

            foreach (var svc in hostedServices)
                services.Remove(svc);

            var authServices = services.Where(s =>
                s.ServiceType.FullName?.Contains("Authentication") == true ||
                s.ServiceType == typeof(IAuthenticationService) ||
                s.ServiceType == typeof(IAuthenticationSchemeProvider) ||
                s.ServiceType.Name?.Contains("JwtBearer") == true)
                .ToList();

            foreach (var service in authServices)
                services.Remove(service);

            services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                            d.ServiceType == typeof(ApplicationDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();

            // Seed test data
            if (!db.Books.Any())
            {
                var author1 = new AuthorEntity { Id = Guid.NewGuid(), FirstName = "Test Author 1", LastName = "Last 1", IsDeleted = false };
                var author2 = new AuthorEntity { Id = Guid.NewGuid(), FirstName = "Test Author 2", LastName = "Last 2", IsDeleted = false };
                db.Authors.AddRange(author1, author2);

                var book1 = new BookEntity { Id = Guid.NewGuid(), Title = "Test Book 1", Year = 2020, IsDeleted = false, Authors = new List<AuthorEntity> { author1, author2 } };
                var book2 = new BookEntity { Id = Guid.NewGuid(), Title = "Test Book 2", Year = 2021, IsDeleted = false, Authors = new List<AuthorEntity> { author1 } };
                db.Books.AddRange(book1, book2);

                db.SaveChanges();
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        base.Dispose();
    }
}
