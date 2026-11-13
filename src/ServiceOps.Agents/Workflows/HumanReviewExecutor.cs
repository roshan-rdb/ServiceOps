using Microsoft.Agents.AI.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class HumanReviewExecutor() : Executor("HumanReview")
{
    [MessageHandler]
    public async ValueTask HandleAsync(
        IncidentWorkflowDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        await context.YieldOutputAsync(
            new IncidentWorkflowResult(
                decision.Context.Incident.Id,
                decision.Context.Assessment!,
                WorkflowRoute.HumanReview
                ),
            cancellationToken
            );
    }
}
