using Microsoft.Agents.AI;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Triage
{
    public sealed class IncidentTriageService(AIAgent agent)
    {
        public async Task<string> AnalyzeAsync(Incident incident, CancellationToken cancellationToken) 
        { 
            AgentResponse response = await agent.RunAsync(
                IncidentTriagePrompt.Build(incident), 
                cancellationToken: cancellationToken); 

            return response.Text; 
        }
    }
}
