using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Techcore_Internship.Data.Utils.Extentions;
using Techcore_Internship.Grpc.ServiceA.Consumers;
using Techcore_Internship.Grpc.ServiceA.Services;

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
            .AddSource("MassTransit")
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Zipkin:Endpoint")
                    ?? "http://zipkin:9411/api/v2/spans");
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter(serviceName)
            .AddMeter("MassTransit")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });
builder.Services.AddCustomOpenTelemetry2();

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
    options.ListenAnyIP(5003);
});

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapGrpcService<ServiceA>();
app.MapGet("/", () => "ServiceA is running on http://localhost:5003");

app.Run();