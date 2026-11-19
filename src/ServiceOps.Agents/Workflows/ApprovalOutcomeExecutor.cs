using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class ApprovalOutcomeExecutor() : Executor("ApprovalOutcome")
{
    private const string PendingDecisionKey = "PendingIncidentDecision";

    [MessageHandler]
    public async ValueTask<IncidentWorkflowResult> HandleAsync(IncidentApprovalResponse response,
        IWorkflowContext context, 
        CancellationToken cancellationToken)
    {
        IncidentWorkflowDecision? decision = await context.ReadStateAsync<IncidentWorkflowDecision>(
            PendingDecisionKey,
            scopeName: WorkflowStateKeys.Scope,
            cancellationToken);

        if(decision is null || decision.Context.Incident.Id != response.IncidentId)
        {
            throw new InvalidOperationException("Approval does not match the pending incident.");
        }
        WorkflowRoute route = response.Approved ? WorkflowRoute.Completed : WorkflowRoute.AwaitingHumanApproval;

        return new IncidentWorkflowResult(response.IncidentId, decision.Context.Assessment!, route);
    }
}
