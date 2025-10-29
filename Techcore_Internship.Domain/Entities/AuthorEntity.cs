using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Domain.Entities;

public class AuthorEntity : IHaveId<Guid>, IHaveIsDeleted
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsDeleted { get; set; }

    public virtual List<BookEntity> Books { get; set; }
}