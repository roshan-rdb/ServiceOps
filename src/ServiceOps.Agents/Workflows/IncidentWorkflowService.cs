using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Checkpointing;
using ModelContextProtocol.Protocol;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

public sealed class IncidentWorkflowService(
    ServiceOpsIncidentWorkflowFactory factory, 
    CheckpointManager checkpointManager,
    FileSystemJsonCheckpointStore checkpointStore)
{
    public async Task<WorkflowExecutionStatus> StartAsync(Incident incident, CancellationToken cancellationToken)
    {
        Workflow workflow = factory.Create();
        string sessionId = incident.Id.ToString("N");
        
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow,
            incident,
            checkpointManager,
            sessionId,
            cancellationToken: cancellationToken);

        await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync(cancellationToken))
        {
            if (workflowEvent is RequestInfoEvent)
            {
                //Visualizing the workflow
                Console.WriteLine("Mermaid String:");
                Console.WriteLine(workflow.ToMermaidString());
                
                return new WorkflowExecutionStatus (incident.Id, "AwaitingHumanApproval");
            }
            if (workflowEvent is WorkflowOutputEvent output)
            {
                IncidentWorkflowResult result = output.As<IncidentWorkflowResult>()
                    ?? throw new InvalidDataException("Invalid workflow output.");

                //Visualizing the workflow
                Console.WriteLine("Mermaid String:");
                Console.WriteLine(workflow.ToMermaidString());

                return new WorkflowExecutionStatus(
                    incident.Id,
                    "Completed",
                    result);
            }
        }
        

        throw new InvalidOperationException("Workflow ended without a terminal state.");
    }

    public async Task<IncidentWorkflowResult> ResumeApprovalAsync(
        Guid incidentId, 
        bool approved,
        string reviewerId,
        string comment,
        CancellationToken cancellationToken)
    {
        Workflow workflow = factory.Create();
        string sessionId = incidentId.ToString("N");

        IEnumerable<CheckpointInfo> checkpoints = await checkpointStore.RetrieveIndexAsync(sessionId);

        CheckpointInfo checkpoint = checkpoints.LastOrDefault() ??
            throw new InvalidOperationException("No resumable workflow exist.");

        await using StreamingRun run = await InProcessExecution.ResumeStreamingAsync(
            workflow,
            checkpoint,
            checkpointManager,
            cancellationToken);

        var approval = new IncidentApprovalResponse(incidentId, approved, reviewerId, comment, DateTimeOffset.UtcNow);

        await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync(cancellationToken))
        {
            if (workflowEvent is not RequestInfoEvent request)
            {
                continue;
            }
            ExternalResponse response = request.Request.CreateResponse(approval);

            await run.SendResponseAsync(response);

            break;
        }

        await foreach (WorkflowEvent workflowEvent in run.WatchStreamAsync(cancellationToken))
        {
            if (workflowEvent is WorkflowOutputEvent output)
            {
                //Visualizing the workflow
                Console.WriteLine("Mermaid String:");
                Console.WriteLine(workflow.ToMermaidString());
                return output.As<IncidentWorkflowResult>() ?? throw new InvalidDataException("Invalid workflow output");
            }
        }

        throw new InvalidOperationException("Resume workflow produced no result");
    }
}
