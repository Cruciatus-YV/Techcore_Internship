namespace Techcore_Internship.Contracts.DTOs.Entities.Book.Events
{
    public record BookCreatedEvent
    {
        public Guid BookId { get; init; }
        public string BookTitle { get; init; } = string.Empty;
        public int Year { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
