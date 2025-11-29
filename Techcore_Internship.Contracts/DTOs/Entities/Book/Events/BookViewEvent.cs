namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Events;

public class BookViewEvent
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public DateTime ViewDate { get; set; }
    public string EventType { get; set; } = string.Empty;
}
