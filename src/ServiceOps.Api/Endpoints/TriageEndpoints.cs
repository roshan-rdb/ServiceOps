using ServiceOps.Agents.Triage;
using ServiceOps.Application.Incidents;
using ServiceOps.Domain.Incidents;
using System.Globalization;

namespace ServiceOps.Api.Endpoints
{
    public sealed record IncidentIntakeRequest(string Title, string Description);
    public sealed record IncedentUpdateRequest(String Update);
    public static class TriageEndpoints
    {
        public static IEndpointRouteBuilder MapTriageEndpoints(this IEndpointRouteBuilder endpoints)
        {
           
            endpoints.MapPost("/api/incidents/triage", async (IncidentIntakeRequest request, IncidentTriageService triage, CancellationToken cancellationToken)
            =>
            {
                if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                {
                    return Results.BadRequest(new { error = "Title and description are required." });

                }
                var incident = new Incident(Guid.NewGuid(), request.Title.Trim(), request.Description.Trim(), DateTimeOffset.UtcNow);
                try
                {
                    TriageExecutionResult triageExecutionResult = await triage.AnalyzeAsync(incident, cancellationToken);
                    IncidentTriageAssessment assessment = triageExecutionResult.Assessment;
                    return Results.Ok(new { incident.Id, Assessment = assessment });
                }
                catch (TimeoutException)
                {
                    return Results.Problem(
                        title: "Incident triage temporarily unavailable.",
                        statusCode: StatusCodes.Status503ServiceUnavailable
                        );
                }
                catch (InvalidDataException ex)
                {
                    return Results.Problem(
                         title: "The triage result could not be validated.",
                         statusCode: StatusCodes.Status502BadGateway
                         );
                }
            });

            endpoints.MapPost("/api/incidents/{incidentId:guid}/updates",
                async (Guid incidentId,
                      IncedentUpdateRequest request,
                      IIncidentConversationStore conversationStore,
                      IncidentTriageService triage,
                      CancellationToken cancellationToken) =>
                {
                    IncidentConversationRecord? conversation =
                        await conversationStore.GetAsync(incidentId, cancellationToken);
                    if (conversation == null)
                    {
                        return Results.NotFound();
                    }
                    if (string.IsNullOrWhiteSpace(request.Update) || request.Update.Trim().Length > 200)
                    {
                        return Results.BadRequest(new { error = "A valid incident Update is required." });
                    }
                    TriageExecutionResult result = await triage.ContinueAsync(
                        conversation.Incidents,
                        conversation.AgentSessionJson,
                        request.Update.Trim(),
                        cancellationToken);

                    await conversationStore.SaveAsync(
                        conversation with
                        {
                            AgentSessionJson = result.AgentSessionJson,
                            UpdatedAtUtc = DateTimeOffset.UtcNow
                        },
                        cancellationToken);
                    return Results.Ok(new { incidentId, Assessment = result.Assessment });
                });

            return endpoints;
        }
    }
}
