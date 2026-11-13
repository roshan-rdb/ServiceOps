using ServiceOps.Application.Incidents;
using ServiceOps.Application.Workflows;

public enum WorkflowRoute
{
    Complete,
    HumanReview
}
public sealed record IncidentWorkflowDecision(IncidentWorkflowContext Context, WorkflowRoute Route);

public sealed record IncidentWorkflowResult(
    Guid IncidentId,
    IncidentTriageAssessment Assessment,
    WorkflowRoute Route);

