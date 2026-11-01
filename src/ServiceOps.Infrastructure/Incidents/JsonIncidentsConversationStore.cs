using ServiceOps.Application.Incidents;
using System.Text.Json;

namespace ServiceOps.Infrastructure.Incidents;

public sealed class JsonIncidentsConversationStore : IIncidentConversationStore
{
    private readonly string _storageDirectory;
    public JsonIncidentsConversationStore(string storageDirectory)
    {
        _storageDirectory = storageDirectory;
        Directory.CreateDirectory(_storageDirectory);
    }
    public async Task<IncidentConversationRecord> GetAsync(Guid incidentId, CancellationToken cancellationToken)
    {
        string filePath = GetFilePath(incidentId);
        if (!File.Exists(filePath))
        {
            return null; // Return null if the file does not exist
        }
        await using FileStream fileStream = File.OpenRead(filePath);

        return await JsonSerializer.DeserializeAsync<IncidentConversationRecord>(fileStream, cancellationToken: cancellationToken);
    }
    public async Task SaveAsync(IncidentConversationRecord conversation, CancellationToken cancellationToken)
    {
        string filePath = GetFilePath(conversation.Incidents.Id);
        
        await using FileStream fileStream = File.Create(filePath);

        await JsonSerializer.SerializeAsync(fileStream, conversation, cancellationToken: cancellationToken);
    }
    private string GetFilePath(Guid incidentId) =>
         Path.Combine(_storageDirectory, $"{incidentId}.json");
    
}
