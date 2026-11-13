using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Incidents;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class AssessmentWorkflowExecutor(AIAgent coordinatorAgent) : Executor("Assessment")
{
    [MessageHandler]
    public async ValueTask<IncidentWorkflowContext> HandleAsync(
        IncidentWorkflowContext context, 
        IWorkflowContext workflowContext,
        CancellationToken cancellationToken)
    {
        string prompt = AssessmentPrompt.Build(context.Incident, 
            context.Diagnostics!, 
            context.Knowledge!, 
            context.Risk!);

        AgentResponse<IncidentTriageAssessment> response =
            await coordinatorAgent.RunAsync<IncidentTriageAssessment>(prompt, cancellationToken: cancellationToken);

        IncidentTriageAssessment assessment = response.Result;
        IReadOnlyList<string> errors = IncidentTriageAssessmentValidator.Validate(assessment);
        if(errors.Count > 0)
        {
            throw new InvalidDataException(string.Join(" ", errors));
        }
        return context with
        {
            Assessment = assessment
        };
    }
}
