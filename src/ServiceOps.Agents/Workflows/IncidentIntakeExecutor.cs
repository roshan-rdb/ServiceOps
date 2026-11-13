using Microsoft.Agents.AI.Workflows;
using ServiceOps.Application.Incidents;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;
internal sealed partial class IncidentIntakeExecutor():Executor("IncidentExecutor")
{
   
    [MessageHandler]
    public ValueTask<IncidentWorkflowContext> HandleAsync(Incident incident, IWorkflowContext context)
    {
        if(incident.Id == Guid.Empty)
        {
            throw new ArgumentException("Incident Id is required");
        }

        if(string.IsNullOrWhiteSpace(incident.Description))
        {
            throw new ArgumentException("Incident Description is required");
        }
        return ValueTask.FromResult(new IncidentWorkflowContext(incident));
    }

    
}
