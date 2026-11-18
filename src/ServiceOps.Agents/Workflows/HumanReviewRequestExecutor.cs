using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Incidents;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class HumanReviewRequestExecutor(): Executor("PrepareHumanReview")
{
    private const string PendingDecisionKey = "PendingIncidentDecision";

    [MessageHandler]
    public async ValueTask<IncidentApprovalRequest> HandleAsync(IncidentWorkflowDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        await context.QueueStateUpdateAsync(
            PendingDecisionKey,
            decision,
            scopeName: WorkflowStateKeys.Scope,
            cancellationToken);

        IncidentTriageAssessment assessment = decision.Context.Assessment 
            ?? throw new InvalidOperationException("Assessment is required before review");

        RiskReview risk = decision.Context.Risk
            ?? throw new InvalidOperationException("Risk review is required before review");

        return new IncidentApprovalRequest(decision.Context.Incident.Id, 
            assessment.Summary, 
            risk.Severity,
            risk.Rationale,
            assessment.Evidence);
    }
}
