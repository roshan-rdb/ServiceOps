using ServiceOps.Application.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Infrastructure.Operations
{
    public sealed class InMemoryDeploymentReader : IDeploymentReader
    {
        private static readonly Dictionary<string, string> Statuses = new(StringComparer.OrdinalIgnoreCase)
        {
            ["identity-api"] = "Degraded",
            ["accounts-api"] = "Healthy",
            ["web-portal"] = "Healthy"
        };
        public Task<DeploymentSnapshot> GetLatestAsync(string serviceName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Statuses.TryGetValue(serviceName, out string? status))
            {
                return Task.FromResult<DeploymentSnapshot>(null!);
            }

            DeploymentSnapshot snapshot = new(
                ServiceName: serviceName,
                DeploymentId: Guid.NewGuid().ToString(),
                Status: status,
                DeployedAtUtc: DateTimeOffset.UtcNow);
            return Task.FromResult<DeploymentSnapshot>(snapshot!); ;
        }
    }
}
