using Grpc.Core;

namespace Techcore_Internship.Grpc.ServiceB.Services
{
    public class ServiceB : Greeter.GreeterBase
    {
        private readonly Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient _serviceAClient;
        private readonly ILogger<ServiceB> _logger;

        public ServiceB(Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient serviceAClient, ILogger<ServiceB> logger)
        {
            _serviceAClient = serviceAClient;
            _logger = logger;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"ServiceB: Received request from {request.Name}");

            var requestToA = new Techcore_Internship.Grpc.ServiceA.HelloRequest { Name = request.Name };
            var responseFromA = await _serviceAClient.SayHelloAsync(requestToA);

            _logger.LogInformation($"ServiceB: Got response from ServiceA: {responseFromA.Message}");

            return new HelloReply
            {
                Message = $"ServiceB says: {responseFromA.Message} + from ServiceB!"
            };
        }
    }
}
