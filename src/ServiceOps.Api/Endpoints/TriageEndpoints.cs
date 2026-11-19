using Microsoft.Agents.AI.Workflows;
using ServiceOps.Agents.Workflows;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Api.Endpoints
{
    public sealed record IncidentIntakeRequest(string Title, string Description);
    public sealed record IncedentUpdateRequest(String Update);
    public sealed record IncidentApprovalInput(bool Approved, string? Comment);
    public static class TriageEndpoints
    {
        public static IEndpointRouteBuilder MapTriageEndpoints(this IEndpointRouteBuilder endpoints)
        {
           
            endpoints.MapPost("/api/incidents/triage", async (
                IncidentIntakeRequest request,
                IncidentWorkflowService workflow,
                CancellationToken cancellationToken)
            =>
            {
                if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                {
                    return Results.BadRequest(new { error = "Title and description are required." });

                }
                var incident = new Incident(Guid.NewGuid(), request.Title.Trim(), request.Description.Trim(), DateTimeOffset.UtcNow);
                try
                {
                    WorkflowExecutionStatus result = await workflow.StartAsync(incident, cancellationToken);
                    
                    if(result.Status == "AwaitingHumanApproval")
                    {
                        return Results.Accepted($"/api/incidents/{incident.Id}", 
                            new { incident.Id, Status = "AwaitingHumanApproval" });
                    }

                    return Results.Ok(
                        new WorkflowExecutionStatus(
                            result.IncidentId,
                            "Completed",
                            result.Result));
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

            endpoints.MapPost("/api/incidents/{incidentId:guid}", async (
                 Guid incidentId,
                 IncidentApprovalInput request,
                 IncidentWorkflowService workflow,
                 CancellationToken cancellationToken)
                =>
            {
                try
                {
                    var incidentWorkflowResult = await workflow.ResumeApprovalAsync(
                        incidentId,
                        request.Approved,
                        Guid.NewGuid().ToString(), 
                        request.Comment, 
                        cancellationToken);

                    return Results.Ok(incidentWorkflowResult);
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

            //endpoints.MapPost("/api/incidents/{incidentId:guid}/updates",
            //    async (Guid incidentId,
            //          IncedentUpdateRequest request,
            //          IIncidentConversationStore conversationStore,
            //          IncidentTriageService triage,
            //          CancellationToken cancellationToken) =>
            //    {
            //        IncidentConversationRecord? conversation =
            //            await conversationStore.GetAsync(incidentId, cancellationToken);
            //        if (conversation == null)
            //        {
            //            return Results.NotFound();
            //        }
            //        if (string.IsNullOrWhiteSpace(request.Update) || request.Update.Trim().Length > 200)
            //        {
            //            return Results.BadRequest(new { error = "A valid incident Update is required." });
            //        }
            //        TriageExecutionResult result = await triage.ContinueAsync(
            //            conversation.Incidents,
            //            conversation.AgentSessionJson,
            //            request.Update.Trim(),
            //            cancellationToken);

            //        await conversationStore.SaveAsync(
            //            conversation with
            //            {
            //                AgentSessionJson = result.AgentSessionJson,
            //                UpdatedAtUtc = DateTimeOffset.UtcNow
            //            },
            //            cancellationToken);
            //        return Results.Ok(new { incidentId, Assessment = result.Assessment });
            //    });

            return endpoints;
        }
    }
}
