using MassTransit;
using Techcore_Internship.Grpc.ServiceB.Consumers;
using Techcore_Internship.Grpc.ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpcClient<Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient>(options =>
{
    options.Address = new Uri("http://grpc-service-a:8080");
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("Cruciatus");
            h.Password("12345qwerty");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

app.MapGrpcService<ServiceB>();
app.MapGet("/", () => "ServiceB is running on http://localhost:8080");

app.MapGet("/test", async (Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient client) =>
{
    var response = await client.SayHelloAsync(new Techcore_Internship.Grpc.ServiceA.HelloRequest { Name = "Test User" });
    return Results.Ok($"Test result: {response.Message}");
});

app.Run();