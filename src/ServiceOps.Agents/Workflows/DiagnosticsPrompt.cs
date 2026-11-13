using ServiceOps.Agents.Triage;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

internal static class DiagnosticsPrompt
{
    public static string Build(Incident incident) => IncidentTriagePrompt.Build(incident);
}
