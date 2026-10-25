using ServiceOps.Application.Operations;

namespace ServiceOps.Infrastructure.Operations
{
    public sealed class InMemoryServiceHealthReader : IServiceHealthReader
    {
        private static readonly Dictionary<string, string> Statuses = new(StringComparer.OrdinalIgnoreCase)
        {
            ["identity-api"] = "Degraded",
            ["accounts-api"] = "Healthy",
            ["web-portal"] = "Healthy"
        };
        Task<ServiceHealthSnapshot> IServiceHealthReader.GetAsync(string serviceName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(!Statuses.TryGetValue(serviceName, out string? status))
            {
                return Task.FromResult<ServiceHealthSnapshot>(null!);
            }

            ServiceHealthSnapshot snapshot = new(
                ServiceName: serviceName,
                Status: status,
                ObservedAtUtc: DateTimeOffset.UtcNow);
            return Task.FromResult<ServiceHealthSnapshot>(snapshot!);
        }
    }
}
