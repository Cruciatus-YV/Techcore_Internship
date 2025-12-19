using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Techcore_Internship.Grpc.ServiceB.Consumers;
using Techcore_Internship.Grpc.ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", context.HostingEnvironment.ApplicationName)
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(
            uri: context.Configuration["Loki:Address"] ?? "http://localhost:3100",
            labels: new[] {
                new LokiLabel { Key = "app", Value = context.HostingEnvironment.ApplicationName },
                new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName }
            });
});

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
            .AddSource("MassTransit")
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
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter(serviceName)
            .AddMeter("MassTransit")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter();
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
    options.ListenAnyIP(5004);
});

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapGrpcService<ServiceB>();
app.MapGet("/", () => "ServiceB is running on http://localhost:5004");

app.MapGet("/test", async (Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient client) =>
{
    var response = await client.SayHelloAsync(new Techcore_Internship.Grpc.ServiceA.HelloRequest { Name = "Test User" });
    return Results.Ok($"Test result: {response.Message}");
});

app.Run();