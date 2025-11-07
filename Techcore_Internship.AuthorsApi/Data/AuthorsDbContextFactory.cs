using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Techcore_Internship.AuthorsApi.Data;

public class AuthorsDbContextFactory : IDesignTimeDbContextFactory<AuthorsDbContext>
{
    public AuthorsDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Techcore_Internship.WebApi");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AuthorsDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Techcore_Internship_Postgres_Connection"));

        return new AuthorsDbContext(optionsBuilder.Options);
    }
}
