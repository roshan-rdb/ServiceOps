using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Domain.Incidents
{
    public sealed record Incident(
         Guid Id,
         string Title,
         string Description,
         DateTimeOffset ReportedAtUtc);
}
