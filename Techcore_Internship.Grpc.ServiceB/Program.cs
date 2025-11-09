using Techcore_Internship.Grpc.ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpcClient<Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient>(options =>
{
    options.Address = new Uri("https://localhost:7001");
});

var app = builder.Build();

app.MapGrpcService<ServiceB>();
app.MapGet("/", () => "ServiceB is running on https://localhost:7002");

app.MapGet("/test", async (Techcore_Internship.Grpc.ServiceA.Greeter.GreeterClient client) =>
{
    var response = await client.SayHelloAsync(new Techcore_Internship.Grpc.ServiceA.HelloRequest { Name = "Test User" });
    return Results.Ok($"Test result: {response.Message}");
});

app.Run();