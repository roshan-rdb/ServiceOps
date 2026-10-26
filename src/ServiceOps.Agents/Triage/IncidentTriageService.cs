using Microsoft.Agents.AI;
using ServiceOps.Domain.Incidents;
using ServiceOps.Application.Incidents;

namespace ServiceOps.Agents.Triage
{
    public sealed class IncidentTriageService(AIAgent agent)
    {
        private static readonly TimeSpan ExecutionTimeout = TimeSpan.FromSeconds(30);
        public async Task<IncidentTriageAssessment> AnalyzeAsync(Incident incident, CancellationToken cancellationToken) 
        { 
            using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(ExecutionTimeout);
            try
            {
                AgentResponse<IncidentTriageAssessment> response = await agent.RunAsync<IncidentTriageAssessment>(
                IncidentTriagePrompt.Build(incident),
                cancellationToken: timeoutSource.Token);

                return Validate(response.Result);
            }
            catch(OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("The incident triage operation exceeded its execution deadline.");
            }
        }

        private static IncidentTriageAssessment Validate(IncidentTriageAssessment assessment)
        {
            IReadOnlyList<string> errors = IncidentTriageAssessmentValidator.Validate(assessment);
            if (errors.Count > 0)
            {
                throw new InvalidDataException($"Agent assessment faild validation: {string.Join(", ", errors)}");
            }
            return assessment;
        }
    }
}
