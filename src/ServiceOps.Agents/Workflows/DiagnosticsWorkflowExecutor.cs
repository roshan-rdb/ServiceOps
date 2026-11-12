using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class DiagnosticsWorkflowExecutor(AIAgent diagnosticsAgent) : Executor("Diagnostics")
{
    protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
    {
        throw new NotImplementedException();
    }

    [MessageHandler]
    public async ValueTask<IncidentWorkflowContext> HandleAsync(
        IncidentWorkflowContext context,
        IWorkflowContext workflowContext,
        CancellationToken cancellationToken)
    {        string prompt = DiagnosticsPrompt.Build(context.Incident);

        AgentResponse<DiagnosticsReport> response = 
            await diagnosticsAgent.RunAsync<DiagnosticsReport>(prompt, cancellationToken: cancellationToken);

        return context with { Diagnostics = response.Result };
    }
}
