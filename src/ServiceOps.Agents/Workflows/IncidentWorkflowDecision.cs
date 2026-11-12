using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

public enum WorkflowRoute
{
    Complete,
    HumanReview
}
public sealed record IncidentWorkflowDecision(IncidentWorkflowContext Context, WorkflowRoute Route);

