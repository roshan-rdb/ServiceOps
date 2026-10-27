using ServiceOps.Domain.Incidents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Agents.Triage
{
    internal static class IncidentTriagePrompt
    {
        public static string Build(Incident incident) => $"""
              Triage the following service incident.

              Incident ID: {incident.Id}  
              Title: {incident.Title}  
              Description: {incident.Description}  
              Reported at: {incident.ReportedAtUtc:O}

              Use available read-only tools only when they provide evidence needed for the assessment.
              Return an assessment based only on the incident and trusted information obtained during this run.
              """;
    }
}
