using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Techcore_Internship.Domain.Entities.Shared;

namespace Techcore_Internship.Domain.Entities
{
    public class BookViewAnalyticsEntity : IHaveId<Guid>
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("bookId")]
        [BsonRepresentation(BsonType.String)]
        public Guid BookId { get; set; }

        [BsonElement("bookTitle")]
        public string BookTitle { get; set; } = string.Empty;

        [BsonElement("viewDate")]
        public DateTime ViewDate { get; set; }

        [BsonElement("eventType")]
        public string EventType { get; set; } = string.Empty;

        [BsonElement("processedAt")]
        public DateTime ProcessedAt { get; set; }
    }
}
