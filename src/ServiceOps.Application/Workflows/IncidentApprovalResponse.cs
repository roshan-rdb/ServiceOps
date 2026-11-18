namespace ServiceOps.Application.Workflows;

public sealed record IncidentApprovalResponse
(
    Guid IncidentId,
    bool Approved,
    string ReviewerId,
    string? Comment,
    DateTimeOffset ReviewedAtUtc);

