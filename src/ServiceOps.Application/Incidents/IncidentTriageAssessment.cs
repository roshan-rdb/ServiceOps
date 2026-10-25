using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ServiceOps.Application.Incidents
{
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public enum IncidentSeverity { Low, Medium, High, Critical }
    public sealed class IncidentTriageAssessment
    {
        public required string Summary { get; init; }
        public required string AffectedService { get; init; }
        public required IncidentSeverity Severity { get; init; }
        public required bool RequiresHumanReview { get; init; }
        public required List<string> Evidence { get; init; }
    }
}
