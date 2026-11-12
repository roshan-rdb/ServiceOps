using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Workflows;

namespace ServiceOps.Agents.Workflows;

internal sealed partial class RiskWorkflowExecutor(AIAgent riskAgent) : Executor("RiskReview")
{
    protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
    {
        throw new NotImplementedException();
    }

    [MessageHandler]
    public async ValueTask<IncidentWorkflowContext> HandleAsync(
        IncidentWorkflowContext context,
        IWorkflowContext workflow,
        CancellationToken cancellationToken
        )
    {
        string prompt = RiskPrompt.Build(
            context.Incident , context.Diagnostics ?? throw new InvalidOperationException
            ("Diagnostics Evidenc is missing"),
            context.Knowledge ?? throw new InvalidOperationException("Knowledge Evidence is missing")
            );

        AgentResponse<RiskReview> response =
            await riskAgent.RunAsync<RiskReview>(prompt, cancellationToken: cancellationToken);

        return context with
        {
            Risk = response.Result
        };
    }
}
