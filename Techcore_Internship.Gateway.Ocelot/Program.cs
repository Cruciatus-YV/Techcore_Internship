using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using Techcore_Internship.Data.Utils.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// OpenTelemetry
builder.Services.AddCustomOpenTelemetry(builder.Configuration);

// health checks
builder.Services.AddHealthChecks();

builder.Services.AddOcelot()
    .AddKubernetes();

var app = builder.Build();

// health check endpoint
app.MapHealthChecks("/healthz");

app.UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");

await app.UseOcelot();

app.Run();