using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Techcore_Internship.AuthorsApi.Domain;

namespace Techcore_Internship.AuthorsApi.Data;

public class AuthorsDbContext : DbContext
{
    public AuthorsDbContext(DbContextOptions<AuthorsDbContext> options)
        : base(options)
    {
    }
    public DbSet<AuthorEntity> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
