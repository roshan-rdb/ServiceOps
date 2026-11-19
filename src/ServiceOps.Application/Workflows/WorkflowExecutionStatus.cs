namespace ServiceOps.Application.Workflows;

public sealed record WorkflowExecutionStatus(
    Guid IncidentId,
    string Status, // "Completed", "AwaitingHumanApproval", "Rejected"
    IncidentWorkflowResult? Result = null);
