using Techcore_Internship.Data.Repositories.Interfaces;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories;

public class BookRepository(ApplicationDbContext dbContext) : GenericRepository<BookEntity, Guid>(dbContext), IBookRepository
{
}
