using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Incidents;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class CompletionExecutor() : Executor("Complete")
{
    [MessageHandler]
    public ValueTask<IncidentWorkflowResult> HandleAsync(
        IncidentWorkflowDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        IncidentTriageAssessment assessment = decision.Context.Assessment!;
        var result = new IncidentWorkflowResult(decision.Context.Incident.Id, assessment, WorkflowRoute.Complete);
        return ValueTask.FromResult(result);
            
    }
}
