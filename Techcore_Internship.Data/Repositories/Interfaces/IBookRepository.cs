using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Repositories.Interfaces;

public interface IBookRepository : IGenericRepository<BookEntity, Guid>
{
}
