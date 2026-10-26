using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceOps.Application.Incidents
{
    public static class IncidentTriageAssessmentValidator
    {
        public static IReadOnlyList<string> Validate(IncidentTriageAssessment assessment)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(assessment.Summary))
            {
                errors.Add("Summary is required.");
            }
            else if (assessment.Summary.Length > 600)
            {
                errors.Add("Summary cannot exceed 600 characters.");
            }

            if (string.IsNullOrWhiteSpace(assessment.AffectedService))
            {
                errors.Add("AffectedService is required.");
            }
            if (assessment.Evidence == null || assessment.Evidence.Count is < 1 or > 6)
            {
                errors.Add("Evidence must contain between 1 and 6 item.");
            }
            if (assessment.Evidence is not null 
                && assessment.Evidence.Any(item => string.IsNullOrWhiteSpace(item) || item.Length > 300))
            {
                errors.Add("Each evidence item must contain 1 to 300 characters.");
            }
            return errors;
        }
    }
}
