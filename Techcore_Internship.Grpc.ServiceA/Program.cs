using MassTransit;
using Techcore_Internship.Grpc.ServiceA.Consumers;
using Techcore_Internship.Grpc.ServiceA.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("Cruciatus");
            h.Password("12345qwerty");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGrpcService<ServiceA>();
app.MapGet("/", () => "ServiceA is running on https://localhost:7001");

app.Run();