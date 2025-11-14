namespace Techcore_Internship.Contracts.DTOs.Entities.Author.Requests
{
    public record ClearAuthorCacheRequest(DateTime Timestamp = default)
    {
        public ClearAuthorCacheRequest() : this(DateTime.UtcNow) { }
    }
}
