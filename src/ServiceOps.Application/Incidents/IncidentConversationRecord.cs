using ServiceOps.Domain.Incidents;

namespace ServiceOps.Application.Incidents;

public sealed record IncidentConversationRecord(
    Incident Incidents,
    string AgentSessionJson,
    DateTimeOffset UpdatedAtUtc
 );

public interface IIncidentConversationStore
{
    Task<IncidentConversationRecord> GetAsync(
        Guid incidentId,
        CancellationToken cancellationToken
        );

    Task SaveAsync(
        IncidentConversationRecord conversation,
        CancellationToken cancellationToken
        );
}

