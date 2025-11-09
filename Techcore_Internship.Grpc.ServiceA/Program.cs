using Techcore_Internship.Grpc.ServiceA.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<ServiceA>();
app.MapGet("/", () => "ServiceA is running on https://localhost:7001");

app.Run();