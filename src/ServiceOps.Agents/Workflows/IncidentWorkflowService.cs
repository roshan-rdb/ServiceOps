using Microsoft.Agents.AI.Workflows;
using ModelContextProtocol.Protocol;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

public sealed class IncidentWorkflowService(ServiceOpsIncidentWorkflowFactory factory, CheckpointManager checkpointManager)
{
    public async Task<WorkflowExecutionStatus> RunAsync(Incident incident, CancellationToken cancellationToken)
    {
        string workflowSessionId = incident.Id.ToString("N");

        Workflow workflow = factory.Create();
        //Run run = await InProcessExecution.RunAsync(
        //    workflow,
        //    incident,
        //    sessionId: incident.Id.ToString("N"), cancellationToken: cancellationToken);

        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow,
            incident,
            checkpointManager,
            workflowSessionId,
            cancellationToken: cancellationToken);

        //WorkflowOutputEvent output = run.NewEvents
        //                            .OfType<WorkflowOutputEvent>()
        //                            .LastOrDefault()
        //                            ?? throw new InvalidOperationException("Workflow produced no output");

        await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync(cancellationToken))
        {
            if (workflowEvent is WorkflowOutputEvent output)
            {
                return new WorkflowExecutionStatus(
                    Completed: true,
                    RequiresApproval: false,
                    Result: output.As<IncidentWorkflowResult>());
            }
            if (workflowEvent is RequestInfoEvent requestEvent)
            {
                return new WorkflowExecutionStatus(
                    Completed: false,
                    RequiresApproval: true,
                    Result: null);
            }
        }

        //Visualizing the workflow
        Console.WriteLine("Mermaid String:");
        Console.WriteLine(workflow.ToMermaidString());

        Console.WriteLine("\n Dot String:");
        Console.WriteLine(workflow.ToDotString());

        return null;
    }
}
