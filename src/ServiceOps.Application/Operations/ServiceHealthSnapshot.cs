using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Application.Operations
{
    public sealed record ServiceHealthSnapshot(
        string ServiceName,
        string Status,
        DateTimeOffset ObservedAtUtc);
    public interface IServiceHealthReader
    {
        Task<ServiceHealthSnapshot> GetAsync(string serviceName, CancellationToken cancellationToken);
    }
}
