using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Techcore_Internship.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestKafkaController : ControllerBase
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<TestKafkaController> _logger;

    public TestKafkaController(IProducer<string, string> producer, ILogger<TestKafkaController> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    [HttpPost("send-test-messages")]
    public async Task<IActionResult> SendTestMessages()
    {
        for (int i = 0; i < 20; i++)
        {
            var message = new Message<string, string>
            {
                Key = $"book-{i % 4}",
                Value = JsonSerializer.Serialize(new
                {
                    BookId = Guid.NewGuid(),
                    BookTitle = $"Test Book {i}",
                    ViewDate = DateTime.UtcNow,
                    EventType = "test_view"
                })
            };

            await _producer.ProduceAsync("book_views", message);
            _logger.LogInformation("Sent test message {MessageId} with key {Key}", i, message.Key);
        }

        return Ok("20 test messages sent");
    }
}