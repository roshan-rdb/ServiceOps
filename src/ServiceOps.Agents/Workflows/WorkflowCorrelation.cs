namespace ServiceOps.Agents.Workflows;

public sealed record WorkflowCorrelation(
    Guid IncidentId,
    DateTimeOffset StartedAtUtc);

//var correlation = new WorkflowCorrelation(Guid.NewGuid(), DateTimeOffset.UtcNow);

