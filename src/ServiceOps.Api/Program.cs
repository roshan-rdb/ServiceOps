using ServiceOps.Agents;
using ServiceOps.Api.Endpoints;
using ServiceOps.Application.Operations;
using ServiceOps.Infrastructure.Operations;
var builder = WebApplication.CreateBuilder(args);

string model = Environment.GetEnvironmentVariable("Model") ?? throw new InvalidOperationException("AI:Model is not  configured.");
string apiKey = Environment.GetEnvironmentVariable("ApiKey") ?? throw new InvalidOperationException("AI:ApiKey is not  configured.");
builder.Services.AddSingleton<IServiceHealthReader, InMemoryServiceHealthReader>();
builder.Services.AddServiceOpsAgents(model, apiKey);

var app = builder.Build();

// Map endpoints
app.MapTriageEndpoints();

app.Run();
