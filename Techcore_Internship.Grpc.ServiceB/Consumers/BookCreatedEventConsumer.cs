using MassTransit;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Events;

namespace Techcore_Internship.Grpc.ServiceB.Consumers
{
    public class BookCreatedEventConsumer : IConsumer<BookCreatedEvent>
    {
        public async Task Consume(ConsumeContext<BookCreatedEvent> context)
        {
            var message = context.Message;

            Console.WriteLine($"ServiceB: Processing new book ID { message.BookId}");

            await Task.Delay(1000);
        }
    }
}
