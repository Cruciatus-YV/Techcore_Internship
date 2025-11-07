using Techcore_Internship.AuthorsApi.Domain.Shared;

namespace Techcore_Internship.AuthorsApi.Domain
{
    public class AuthorEntity : IHaveId<Guid>, IHaveIsDeleted
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
