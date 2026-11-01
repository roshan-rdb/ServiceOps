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

        public static string BuildUpdate(Incident incident, string update) => $"""
              Reassess incident {incident.Id} using the new operational update.
              
              New Update: {update}

              Preserve conclusions that remain supported, revise conclusions when the new evidence requires it,
              and use approved knowledge or read-only tools when relevant.
              """;
    }
}
