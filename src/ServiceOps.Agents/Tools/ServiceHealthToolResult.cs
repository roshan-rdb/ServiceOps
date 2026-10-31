namespace ServiceOps.Agents.Tools;

public sealed record ServiceHealthToolResult(
    bool Success,
    string Code,
    string Message,
    string? ServiceName = null,
    string? Status=null,
    DateTimeOffset? ObservedAtUtc = null);