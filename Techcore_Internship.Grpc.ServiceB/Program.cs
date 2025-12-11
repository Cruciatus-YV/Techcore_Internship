using MassTransit;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Techcore_Internship.Grpc.ServiceB.Consumers;
using Techcore_Internship.Grpc.ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry
var serviceName = "GrpcServiceB";
var serviceVersion = "1.0.0";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
            ["service.type"] = "grpc-service"
        }))
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(serviceName)
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation()
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Zipkin:Endpoint")
                    ?? "http://zipkin:9411/api/v2/spans");
            });
    });

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