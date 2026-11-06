namespace ServiceOps.Application.Operations;

public sealed record DeploymentSnapshot(
    string ServiceName,
    string DeploymentId,
    string Status,
    DateTimeOffset DeployedAtUtc);

public interface IDeploymentReader
{
    Task<DeploymentSnapshot> GetLatestAsync(string serviceName, CancellationToken cancellationToken);
}

