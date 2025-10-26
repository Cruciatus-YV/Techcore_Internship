using Microsoft.EntityFrameworkCore;

namespace Techcore_Internship.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets здесь
    // public DbSet<User> Users { get; set; }
}
