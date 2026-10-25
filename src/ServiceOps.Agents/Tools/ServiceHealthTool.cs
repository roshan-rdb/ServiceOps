using System.ComponentModel;
using ServiceOps.Application.Operations;

namespace ServiceOps.Agents.Tools
{
    public sealed class ServiceHealthTool(IServiceHealthReader healthReader)
    {
        [Description("Get the current read-only health status for an approved ServiceOps service.")]
        public async Task<ServiceHealthToolResult> GetServiceHealthAsync(
            [Description("Approved service identifier, such as identity-api or accounts-api.")]
            string serviceName, 
            CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(serviceName) || serviceName.Length > 80)
            {
                return new(false, "invalid_service_name.", "A valid service identifier is required.");
            }

            ServiceHealthSnapshot? snapshot = await healthReader.GetAsync(serviceName, cancellationToken);
            if (snapshot is null)
            {
                return new ServiceHealthToolResult(
                    Success: false,
                    Code: "service-not-found",
                    Message: "The requested service is not available to this tool.");
            }

            return new ServiceHealthToolResult(
                Success: true,
                Code: "ok",
                Message: $"Service health retrieved successfully.",
                ServiceName: snapshot.ServiceName,
                Status: snapshot.Status,
                ObservedAtUtc: snapshot.ObservedAtUtc);
        }

    }
}
