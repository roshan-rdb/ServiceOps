using Microsoft.Agents.AI.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class HumanReviewExecutor() : Executor("HumanReview")
{
    [MessageHandler]
    public ValueTask<IncidentWorkflowResult> HandleAsync(
        IncidentWorkflowDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        var result = new IncidentWorkflowResult(
            decision.Context.Incident.Id,
            decision.Context.Assessment!,
            WorkflowRoute.HumanReview
            );

        return ValueTask.FromResult(result);
    }
}
