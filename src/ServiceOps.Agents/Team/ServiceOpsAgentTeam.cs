using Microsoft.Agents.AI;

namespace ServiceOps.Agents.Team;

public sealed record ServiceOpsAgentTeam(
    AIAgent Coordinator,
    AIAgent Knowledge,
    AIAgent Diagnostics,
    AIAgent Risk
);

