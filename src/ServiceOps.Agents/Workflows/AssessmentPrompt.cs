using ServiceOps.Application.Workflows;
using ServiceOps.Domain.Incidents;

namespace ServiceOps.Agents.Workflows;

internal static class AssessmentPrompt
{
    public static string Build(
        Incident incident,
        DiagnosticsReport diagnostics,
        KnowledgeReport knowledge,
        RiskReview risk) => 
            $"""{incident.ToString()} {diagnostics.ToString()} {knowledge.ToString()} {risk.ToString()} """;
}
