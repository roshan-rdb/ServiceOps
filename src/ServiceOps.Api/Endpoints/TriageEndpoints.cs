using ServiceOps.Agents.Triage;
using ServiceOps.Application.Incidents;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Api.Endpoints
{
    public sealed record IncidentIntakeRequest(string Title, string Description);
    public static class TriageEndpoints
    {
        public static IEndpointRouteBuilder MapTriageEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/incidents/triage")
                        .WithTags("Triage");
            group.MapPost("/", async (IncidentIntakeRequest request, IncidentTriageService triage, CancellationToken cancellationToken)
            =>
            {
                if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                {
                    return Results.BadRequest(new { error = "Title and description are  required." });

                }
                var incident = new Incident(Guid.NewGuid(), request.Title.Trim(), request.Description.Trim(), DateTimeOffset.UtcNow);
                try
                {
                    IncidentTriageAssessment assessment = await triage.AnalyzeAsync(incident, cancellationToken);
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
            return endpoints;
        }
    }
}
