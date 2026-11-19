namespace ServiceOps.Application.Workflows;

public sealed record WorkflowExecutionStatus(
   // Guid IncidentId,
    //string Status,
    bool Completed,
    bool RequiresApproval,
    IncidentWorkflowResult? Result=null);
