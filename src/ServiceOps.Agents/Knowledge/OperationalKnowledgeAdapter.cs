using Microsoft.Agents.AI;
using ServiceOps.Application.Knowledge;

namespace ServiceOps.Agents.Knowledge;

public sealed class OperationalKnowledgeAdapter(IOperationalKnowledgeSearch search)
{
    public async Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        IReadOnlyList<KnowledgeExcerpt> excerpts = await search.SearchAsync(query, maximumResults: 3, cancellationToken);
        return excerpts.Select(excerpt => new TextSearchProvider.TextSearchResult
        {
            SourceName = excerpt.Title,
            SourceLink = excerpt.Source,
            Text = excerpt.Text
        });
    }

}
