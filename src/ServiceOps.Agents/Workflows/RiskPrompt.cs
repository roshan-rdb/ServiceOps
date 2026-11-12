using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;
internal static class RiskPrompt
{
    public static string Build(Incident incident, DiagnosticsReport diagnostics, KnowledgeReport knowledge) =>
        $"""{incident.ToString()} {diagnostics.ToString()} {knowledge.ToString()} """;

}
