using ModelContextProtocol.Server;
using ServiceOps.Application.Operations;
using System.ComponentModel;

namespace ServiceOps.OperationsMcp;

[McpServerToolType]
public sealed class OperationsTools(IDeploymentReader deploymentReader)
{
    [McpServerTool]
    [Description("Get the most recent deployment for an approved service.")]
    public async Task<DeploymentSnapshot?> GetRecentDeploymentAsync(
        [Description("Approved ServiceOps service identifier.")]
        string serviceName, 
        CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(serviceName) || serviceName.Length > 80)
        {
            return null;
        }
        return await deploymentReader.GetLatestAsync(serviceName.Trim(), cancellationToken);
    }
}
