using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Incidents;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class CompletionExecutor() : Executor("Complete")
{
    [MessageHandler]
    public async ValueTask HandleAsync(
        IncidentWorkflowDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        IncidentTriageAssessment assessment = decision.Context.Assessment!;
        await context.YieldOutputAsync(
            new IncidentWorkflowResult
            (
                decision.Context.Incident.Id,
                assessment,
                WorkflowRoute.Complete), 
            cancellationToken);
    }
}
