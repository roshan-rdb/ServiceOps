using ModelContextProtocol.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Agents.Mcp;

public static class McpToolPolicy
{
    private static readonly HashSet<string> AllowedTools = new(StringComparer.Ordinal)
    {
        "get_recent_deployment",
        "get_change_window"
    };

    public static IReadOnlyList<McpClientTool> Filter(IEnumerable<McpClientTool> tools)
    {
        return tools.Where(t => AllowedTools.Contains(t.Name)).ToArray();
    }
}
