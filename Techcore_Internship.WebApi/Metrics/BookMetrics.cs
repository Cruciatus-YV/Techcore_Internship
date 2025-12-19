using System.Diagnostics.Metrics;

namespace Techcore_Internship.WebApi.Metrics
{
    public static class BookMetrics
    {
        private static readonly Meter _meter = new Meter("BookService", "1.0.0");

        public static readonly Counter<int> BookCreatedCounter =
            _meter.CreateCounter<int>("Counter", "Book", "Created book's count");
    }
}
