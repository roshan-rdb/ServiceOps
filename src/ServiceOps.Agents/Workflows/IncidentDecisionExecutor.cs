using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Incidents;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class IncidentDecisionExecutor() : Executor("Decision")
{
    [MessageHandler]
    public ValueTask<IncidentWorkflowDecision> HandleAsync(
        IncidentWorkflowContext context,
        IWorkflowContext workflowContext)
    {
        RiskReview risk = context.Risk ?? 
            throw new InvalidOperationException("Risk review is required");

        IncidentTriageAssessment assessment = context.Assessment ?? 
            throw new InvalidOperationException("Assessment is required");

        WorkflowRoute route = risk.RequiresHumamReview || assessment.RequiresHumanReview
                                || risk.Severity == IncidentSeverity.Critical
                                ? WorkflowRoute.HumanReview : WorkflowRoute.Complete;

        return ValueTask.FromResult(
            new IncidentWorkflowDecision(context, route)
            );
    }
}
