using ServiceOps.Agents.Triage;
using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;
internal static class RiskPrompt
{
    public static string Build(Incident incident, DiagnosticsReport diagnostics, KnowledgeReport knowledge) =>
        $"""{IncidentTriagePrompt.Build(incident)} {diagnostics.Summary} {knowledge.Summary} """;

}
