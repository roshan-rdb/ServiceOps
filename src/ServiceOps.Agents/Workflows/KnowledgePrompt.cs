using ServiceOps.Agents.Triage;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

internal static class KnowledgePrompt
{
    public static string Build(Incident incident, DiagnosticsReport diagnostics) => $"""{IncidentTriagePrompt.Build(incident)} {diagnostics.Summary}""";
}
