namespace Techcore_Internship.Domain.Entities.Shared;

public interface IHaveId<TId>
{
    TId Id { get; set; }
}
