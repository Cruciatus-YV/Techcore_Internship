using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// health checks
builder.Services.AddHealthChecks();

builder.Services.AddOcelot()
    .AddKubernetes();

var app = builder.Build();

// health check endpoint
app.MapHealthChecks("/healthz");

await app.UseOcelot();

app.Run();