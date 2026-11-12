using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Knowledge;

internal static class KnowledgePrompt
{
    public static string Build(Incident incident, DiagnosticsReport diagnostics) => $"""{incident.ToString()} {diagnostics.ToString()}"""
}
