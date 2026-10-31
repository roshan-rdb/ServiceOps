using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Application.Knowledge
{
    public sealed record KnowledgeExcerpt(
        string Title,
        string Source,
        string Text);
    public interface IOperationalKnowledgeSearch
    {
        Task<IReadOnlyList<KnowledgeExcerpt>> SearchAsync(
            string query,
            int maximumResults,
            CancellationToken cancellationToken);
    }
}
