using Microsoft.Agents.AI;
using ServiceOps.Application.Incidents;
using ServiceOps.Domain.Incidents;
using System.Text.Json;

namespace ServiceOps.Agents.Triage
{
    public sealed class IncidentTriageService(AIAgent agent)
    {
        private static readonly TimeSpan ExecutionTimeout = TimeSpan.FromSeconds(30);
        public async Task<TriageExecutionResult> AnalyzeAsync(Incident incident, CancellationToken cancellationToken)
        {
            using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(ExecutionTimeout);
            try
            {
                AgentSession session = await agent.CreateSessionAsync(cancellationToken);

                AgentResponse<IncidentTriageAssessment> response = await agent.RunAsync<IncidentTriageAssessment>
                (
                    IncidentTriagePrompt.Build(incident),
                    session,
                    cancellationToken: timeoutSource.Token
                 );
                JsonElement serialized = await agent.SerializeSessionAsync(
                    session,
                    cancellationToken: cancellationToken
                    );

                return new TriageExecutionResult
                    (
                        Validate(response.Result),
                        serialized.GetRawText()
                      );
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("The incident triage operation exceeded its execution deadline.");
            }
        }
        public async Task<TriageExecutionResult> ContinueAsync(
            Incident incident,
            string sessionJson,
            string update,
            CancellationToken cancellationToken
            )
        {
            AgentSession session = await RestoreSessionAsync(sessionJson, cancellationToken);
            return await RunWithSessionAsync(incident, session, update, cancellationToken);
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

        private async Task<AgentSession> RestoreSessionAsync(string sessionJson, CancellationToken cancellationToken)
        {
            using JsonDocument document = JsonDocument.Parse(sessionJson);
            return await agent.DeserializeSessionAsync(document.RootElement.Clone(),
                cancellationToken: cancellationToken);
        }

        private async Task<TriageExecutionResult> RunWithSessionAsync(
            Incident incident, 
            AgentSession session,
            string update, 
            CancellationToken cancellationToken)
        {
            using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(ExecutionTimeout);
            AgentResponse<IncidentTriageAssessment> response = await agent.RunAsync<IncidentTriageAssessment>
                (
                    IncidentTriagePrompt.Build(incident),
                    session,
                    cancellationToken: timeoutSource.Token
                 );
            JsonElement serialized = await agent.SerializeSessionAsync(
                session,
                cancellationToken: cancellationToken
                );

            return new TriageExecutionResult(Validate(response.Result),serialized.GetRawText());
        }

    }
}
