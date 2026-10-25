using Microsoft.Agents.AI;
using ServiceOps.Domain.Incidents;
using ServiceOps.Application.Incidents;

namespace ServiceOps.Agents.Triage
{
    public sealed class IncidentTriageService(AIAgent agent)
    {
        public async Task<IncidentTriageAssessment> AnalyzeAsync(Incident incident, CancellationToken cancellationToken) 
        { 
            AgentResponse<IncidentTriageAssessment> response = await agent.RunAsync<IncidentTriageAssessment>(
                IncidentTriagePrompt.Build(incident), 
                cancellationToken: cancellationToken); 

            return response.Result; 
        }
    }
}
