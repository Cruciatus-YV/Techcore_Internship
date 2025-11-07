namespace Techcore_Internship.AuthorsApi.Domain.Shared;

public interface IHaveId<TId>
{
    TId Id { get; set; }
}
