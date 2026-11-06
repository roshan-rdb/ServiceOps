
using ModelContextProtocol.Client;
namespace ServiceOps.Agents.Mcp;

public static class ServiceOpsMcpClient
{
    public static Task<McpClient> CreateLocalAsync(CancellationToken cancellationToken)
    {
        var transport = new StdioClientTransport(
            new StdioClientTransportOptions
            {
                Name = "ServiceOps Operations Mcp",
                Command = "dotnet",
                Arguments = [
                    "run",
                    "--project",
                    "../ServiceOps.OperationsMcp/ServiceOps.OperationsMcp.csproj",
                    ]
            });

        return McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
    }
}
