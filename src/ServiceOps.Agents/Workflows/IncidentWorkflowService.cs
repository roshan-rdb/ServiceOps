using Microsoft.Agents.AI.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

public sealed class IncidentWorkflowService(ServiceOpsIncidentWorkflowFactory factory)
{
    public async Task<IncidentWorkflowResult> RunAsync(Incident incident, CancellationToken cancellationToken)
    {
        Workflow workflow = factory.Create();
        Run run = await InProcessExecution.RunAsync(
            workflow,
            incident,
            sessionId: incident.Id.ToString("N"), cancellationToken: cancellationToken);

        WorkflowOutputEvent output = run.NewEvents
                                    .OfType<WorkflowOutputEvent>()
                                    .LastOrDefault()
                                    ?? throw new InvalidOperationException("Workflow produced no output");

        //Visualizing the workflow
        Console.WriteLine("Mermaid String:");
        Console.WriteLine(workflow.ToMermaidString());

        Console.WriteLine("\n Dot String:");
        Console.WriteLine(workflow.ToDotString());

        return output.As<IncidentWorkflowResult>() ?? throw new InvalidDataException("Unexpected workflow output.");
    }
}
