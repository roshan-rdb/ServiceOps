using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class KnowledgeWorkflowExecutor(AIAgent KnowledgeAgent) : Executor("Executor")
{

    [MessageHandler]
    public async ValueTask<IncidentWorkflowContext> HandleAsync(
        IncidentWorkflowContext context,
        IWorkflowContext workflowContext,
        CancellationToken cancellationToken)
    {
        string prompt = KnowledgePrompt.Build(context.Incident, context.Diagnostics!);
        AgentResponse<KnowledgeReport> response =
            await KnowledgeAgent.RunAsync<KnowledgeReport>(prompt, cancellationToken: cancellationToken);
        return context with { Knowledge = response.Result };
    }
}