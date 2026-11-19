using Microsoft.Agents.AI.Workflows;
using ModelContextProtocol.Protocol;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

public sealed class IncidentWorkflowService(ServiceOpsIncidentWorkflowFactory factory, CheckpointManager checkpointManager)
{
    public async Task<WorkflowExecutionStatus> StartAsync(Incident incident, CancellationToken cancellationToken)
    {
        Workflow workflow = factory.Create();
        string sessionId = incident.Id.ToString("N");

        //Run run = await InProcessExecution.RunAsync(
        //    workflow,
        //    incident,
        //    sessionId: incident.Id.ToString("N"), cancellationToken: cancellationToken);

        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow,
            incident,
            checkpointManager,
            sessionId,
            cancellationToken: cancellationToken);

        //WorkflowOutputEvent output = run.NewEvents
        //                            .OfType<WorkflowOutputEvent>()
        //                            .LastOrDefault()
        //                            ?? throw new InvalidOperationException("Workflow produced no output");

        await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync(cancellationToken))
        {
            if (workflowEvent is RequestInfoEvent)
            {
                return new WorkflowExecutionStatus (incident.Id, "AwaitingHumanApproval");
            }
            if (workflowEvent is WorkflowOutputEvent output)
            {
                IncidentWorkflowResult result = output.As<IncidentWorkflowResult>()
                    ?? throw new InvalidDataException("Invalid workflow output.");

                return new WorkflowExecutionStatus(
                    incident.Id,
                    "Completed",
                    result);
            }
        }
        //Visualizing the workflow
        Console.WriteLine("Mermaid String:");
        Console.WriteLine(workflow.ToMermaidString());

        Console.WriteLine("\n Dot String:");
        Console.WriteLine(workflow.ToDotString());

        throw new InvalidOperationException("Workflow ended without a terminal state.");
    }
}
