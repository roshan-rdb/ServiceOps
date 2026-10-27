using ServiceOps.Application.Incidents;

namespace ServiceOps.Tests
{
    public sealed class IncidentTriageAssessmentValidatorTests
    {
        [Fact]
        public void Validate_RejectsAssessmentWithoutEvidence()
        {
            var assessment = new IncidentTriageAssessment
            {
                Summary = "Login failures detected.",
                AffectedService = "identtity-api",
                Severity = IncidentSeverity.High,
                RequiresHumanReview = false,
                Evidence = []
            };
            IReadOnlyList<string> errors = IncidentTriageAssessmentValidator.Validate(assessment);
            Assert.Contains(errors, error=> error.Contains("Evidence"));
        }
    }
            
}
