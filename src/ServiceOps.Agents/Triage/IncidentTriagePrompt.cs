using ServiceOps.Domain.Incidents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Agents.Triage
{
    internal static class IncidentTriagePrompt
    {
        public static string Build(Incident incident) => $"""
              Review this service incident.
              Incident ID: {incident.Id}  
              Title: {incident.Title}  
              Description: {incident.Description}  
              Reported at: {incident.ReportedAtUtc:O}  
              Provide a preliminary operational assessment.
              """;
    }
}
