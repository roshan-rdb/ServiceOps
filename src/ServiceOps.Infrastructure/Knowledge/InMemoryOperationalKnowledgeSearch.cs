using ServiceOps.Application.Knowledge;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Infrastructure.Knowledge;

public class InMemoryOperationalKnowledgeSearch : IOperationalKnowledgeSearch
{
    private static readonly KnowledgeExcerpt[] Documents =
        [
            new (
                    "Identity Service Login Failure Runbook",
                    "runbook://identity/login-failures",
                    """
                    Intermittent login failures after deployment may indicate configuration drift,
                    dependency failure, or unhealthy instances.
                    Confirm service health and recent deployment status before remediation.
                    Escalate Critical incidents before making production changes
                    """
                ),
            new (
                "Incident Severity Standard",
                "policy://operational/severity",
                """
                High severity applies when a production service is materially degraded.
                Critical severity requires widespread business impact, severe data risk,
                or loss of a business-critical capability
                """
                )
        ];

    public Task<IReadOnlyList<KnowledgeExcerpt>> SearchAsync(string query, 
        int maximumResults, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string[] terms = query.Split([' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries)
                               .Where(term => term.Length >= 4)
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToArray();
        IReadOnlyList<KnowledgeExcerpt> results =
            Documents.Select(document => new
            {
                Document = document,
                Score = terms.Count(term => document.Text.Contains(term, StringComparison.OrdinalIgnoreCase)
                        || document.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                )
            })
            .Where(item => item.Score > 0)
            .OrderByDescending(item => item.Score)
            .Take(Math.Clamp(maximumResults, 1, 5))
            .Select(item => item.Document)
            .ToArray();

        return Task.FromResult(results);    
    }
}
