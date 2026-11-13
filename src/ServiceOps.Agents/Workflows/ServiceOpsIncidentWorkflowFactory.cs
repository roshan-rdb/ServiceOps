using Microsoft.Agents.AI.Workflows;
using ServiceOps.Agents.Team;

namespace ServiceOps.Agents.Workflows;

public class ServiceOpsIncidentWorkflowFactory
{
    private readonly ServiceOpsAgentTeam _team;
    public ServiceOpsIncidentWorkflowFactory(ServiceOpsAgentTeam team)
    {
        _team = team;
    }
    public static Func<object?, bool> RouteIs(WorkflowRoute route) =>
    message => message is IncidentWorkflowDecision decision && decision.Route == route;

    public Workflow Create()
    {
        // Initiate Executor
        var intake = new IncidentIntakeExecutor();

        var diagnostics = new DiagnosticsWorkflowExecutor(_team.Diagnostics);

        var knowledge = new KnowledgeWorkflowExecutor(_team.Knowledge);

        var risk = new RiskWorkflowExecutor(_team.Risk);

        // Synthesis and deterministic executor
        var assessment = new AssessmentWorkflowExecutor(_team.Coordinator);

        var decision = new IncidentDecisionExecutor();

        var complete = new CompletionExecutor();

        var review = new HumanReviewExecutor();

        // Build madnatory execution chain
        WorkflowBuilder builder = new WorkflowBuilder(intake)
                                    .AddEdge(intake, diagnostics)
                                    .AddEdge(diagnostics, knowledge)
                                    .AddEdge(knowledge, risk)
                                    .AddEdge(risk, assessment)
                                    .AddEdge(assessment, decision);

        //Deterministic routing
        builder.AddSwitch(decision,
            switchBuilder => switchBuilder.AddCase(RouteIs(WorkflowRoute.Complete), complete)
                                          .AddCase(RouteIs(WorkflowRoute.HumanReview), review)
                                          .WithDefault(review));

        return builder.WithName("serviceops-incident-triage")
                       .WithOutputFrom(complete, review)
                      //.WithOutputFrom<IncidentWorkflowResult>(complete, review)
                       .Build();
    }
}
