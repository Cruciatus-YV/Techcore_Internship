using MassTransit;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Events;

namespace Techcore_Internship.Grpc.ServiceA.Consumers
{
    public class BookCreatedEventConsumer : IConsumer<BookCreatedEvent>
    {
        public async Task Consume(ConsumeContext<BookCreatedEvent> context)
        {
            var message = context.Message;

            Console.WriteLine($"ServiceA: Book '{message.BookTitle}' created in {message.Year}");

            await Task.Delay(1000);
        }
    }
}
