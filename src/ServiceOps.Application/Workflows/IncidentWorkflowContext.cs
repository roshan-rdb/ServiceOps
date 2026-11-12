using ServiceOps.Application.Incidents;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Application.Workflows;

public sealed record IncidentWorkflowContext(
    Incident Incident,
    DiagnosticsReport? Diagnostics = null,
    KnowledgeReport? Knowledge = null,
    RiskReview? Risk = null,
    IncidentTriageAssessment? Assessment = null
    );
public sealed record DiagnosticsReport(
    string Summary,
    IReadOnlyList<string> Evidence);
public sealed record KnowledgeReport(
    string Summary,
    IReadOnlyList<string> Evidence);
public sealed record RiskReview( 
    IncidentSeverity Severity,
    bool RequiresHumamReview,
    string Rationale
   );

