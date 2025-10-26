using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories
{
    public class AuthorRepository(ApplicationDbContext dbContext) : GenericRepository<AuthorEntity, Guid>(dbContext), IAuthorRepository
    {
    }
}
