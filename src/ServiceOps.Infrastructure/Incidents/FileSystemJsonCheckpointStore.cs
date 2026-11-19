using ServiceOps.Application.Incidents;
using System.Text.Json;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Checkpointing;

namespace ServiceOps.Infrastructure.Incidents;

public sealed class FileSystemJsonCheckpointStore : ICheckpointStore<JsonElement>
{
    private readonly string _checkpointDirectory;
    private FileSystemJsonCheckpointStore _checkpointStore;
    public FileSystemJsonCheckpointStore(string checkpointDirectory)
    {
        _checkpointDirectory = checkpointDirectory;
        Directory.CreateDirectory(_checkpointDirectory);
        _checkpointStore = new FileSystemJsonCheckpointStore(_checkpointDirectory);
    }

    public ValueTask<CheckpointInfo> CreateCheckpointAsync(string sessionId, JsonElement value, CheckpointInfo? parent = null)
    {
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var checkpointManager = CheckpointManager.CreateJson(_checkpointStore, jsonOptions);
        return new ValueTask<CheckpointInfo>(new CheckpointInfo(sessionId, string.Empty));
    }

    public Task<IncidentConversationRecord> GetAsync(Guid incidentId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<JsonElement> RetrieveCheckpointAsync(string sessionId, CheckpointInfo key)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IEnumerable<CheckpointInfo>> RetrieveIndexAsync(string sessionId, CheckpointInfo? withParent = null)
    {
        throw new NotImplementedException();
    }
}
