using ServiceOps.Application.Incidents;

namespace ServiceOps.Application.Workflows;

public sealed record IncidentApprovalRequest
(
    Guid IncidentId,
    string Summary,
    IncidentSeverity Severity,
    string RiskRationale,
    IReadOnlyList<string> Evidence
);
