using Grpc.Core;

namespace Techcore_Internship.Grpc.ServiceA.Services
{
    public class ServiceA : Greeter.GreeterBase
    {
        private readonly ILogger<ServiceA> _logger;
        public ServiceA(ILogger<ServiceA> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"ServiceA: Received request from {request.Name}");

            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
