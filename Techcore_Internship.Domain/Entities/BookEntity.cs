using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Domain.Entities;

public class BookEntity : IHaveId<Guid>, IHaveIsDeleted
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public bool IsDeleted { get; set; }

    public virtual List<AuthorEntity> Authors { get; set; }
}