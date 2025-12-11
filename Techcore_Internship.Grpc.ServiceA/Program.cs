using MassTransit;
using Techcore_Internship.Grpc.ServiceA.Consumers;
using Techcore_Internship.Grpc.ServiceA.Services;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry
var serviceName = "GrpcServiceA";
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
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Zipkin:Endpoint")
                    ?? "http://zipkin:9411/api/v2/spans");
            });
    });

builder.Services.AddGrpc();

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

app.MapGrpcService<ServiceA>();
app.MapGet("/", () => "ServiceA is running on http://localhost:8080");

app.Run();