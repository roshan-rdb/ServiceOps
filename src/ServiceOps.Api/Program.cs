using ServiceOps.Agents;
using ServiceOps.Api.Endpoints;
using ServiceOps.Application.Incidents;
using ServiceOps.Application.Knowledge;
using ServiceOps.Application.Operations;
using ServiceOps.Infrastructure.Incidents;
using ServiceOps.Infrastructure.Operations;
using ServiceOps.Infrastructure.Knowledge;
var builder = WebApplication.CreateBuilder(args);

string model = Environment.GetEnvironmentVariable("Model") ?? throw new InvalidOperationException("AI:Model is not  configured.");
string apiKey = Environment.GetEnvironmentVariable("ApiKey") ?? throw new InvalidOperationException("AI:ApiKey is not  configured.");
builder.Services.AddSingleton<IServiceHealthReader, InMemoryServiceHealthReader>();
builder.Services.AddServiceOpsAgents(model, apiKey);
builder.Services.AddSingleton<IIncidentConversationStore>(
    new JsonIncidentsConversationStore(
        Path.Combine(
            builder.Environment.ContentRootPath,
            "App_Data",
            "incidents")
        )
    );
builder.Services.AddSingleton<IOperationalKnowledgeSearch, InMemoryOperationalKnowledgeSearch>();
var app = builder.Build();

// Map endpoints
app.MapTriageEndpoints();

app.Run();
