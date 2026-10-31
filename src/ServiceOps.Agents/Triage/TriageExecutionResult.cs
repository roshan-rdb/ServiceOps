using ServiceOps.Application.Incidents;

namespace ServiceOps.Agents.Triage;

public sealed record TriageExecutionResult
(
    IncidentTriageAssessment Assessment,
    string AgentSessionJson
);

