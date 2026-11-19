using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Checkpointing;
using ModelContextProtocol.Client;
using ServiceOps.Agents;
using ServiceOps.Agents.Mcp;
using ServiceOps.Api.Endpoints;
using ServiceOps.Application.Incidents;
using ServiceOps.Application.Knowledge;
using ServiceOps.Application.Operations;
using ServiceOps.Infrastructure.Incidents;
using ServiceOps.Infrastructure.Knowledge;
using ServiceOps.Infrastructure.Operations;
using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);

string model = Environment.GetEnvironmentVariable("Model") ?? throw new InvalidOperationException("AI:Model is not  configured.");
string apiKey = Environment.GetEnvironmentVariable("ApiKey") ?? throw new InvalidOperationException("AI:ApiKey is not  configured.");
builder.Services.AddSingleton<IServiceHealthReader, InMemoryServiceHealthReader>();

McpClient operationsMcpClient = await ServiceOpsMcpClient.CreateLocalAsync(CancellationToken.None);
IList<McpClientTool> discoverdTools = await operationsMcpClient.ListToolsAsync();
IReadOnlyList<McpClientTool> approvedMcpTools = McpToolPolicy.Filter(discoverdTools);
builder.Services.AddServiceOpsAgents(model, apiKey, approvedMcpTools);

builder.Services.AddSingleton<IIncidentConversationStore>(
    new JsonIncidentsConversationStore(
        Path.Combine(
            builder.Environment.ContentRootPath,
            "App_Data",
            "incidents")
        )
    );


DirectoryInfo checkpointDirectory = new(
        Path.Combine(
        builder.Environment.ContentRootPath,
        "App_Data",
        "workflow-checkpoints"));
checkpointDirectory.Create();

var checkpoitStore = new FileSystemJsonCheckpointStore(checkpointDirectory);
var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
CheckpointManager checkpointManager = CheckpointManager.CreateJson(checkpoitStore, jsonOptions);
builder.Services.AddSingleton(checkpointManager);
builder.Services.AddSingleton<ICheckpointStore<JsonElement>>(checkpoitStore);

builder.Services.AddSingleton<IOperationalKnowledgeSearch, InMemoryOperationalKnowledgeSearch>();
var app = builder.Build();

// Map endpoints
app.MapTriageEndpoints();

app.Run();
