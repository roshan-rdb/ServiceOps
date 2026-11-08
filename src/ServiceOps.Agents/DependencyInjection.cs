using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using OpenAI;
using ServiceOps.Agents.Diagnostics;
using ServiceOps.Agents.Knowledge;
using ServiceOps.Agents.Risk;
using ServiceOps.Agents.Team;
using ServiceOps.Agents.Tools;
using ServiceOps.Agents.Triage;
using ServiceOps.Application.Knowledge;
using ServiceOps.Application.Operations;

namespace ServiceOps.Agents
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceOpsAgents(this IServiceCollection services, 
            string model, 
            string apiKey, 
            IReadOnlyList<McpClientTool> mcpTools)
        {
            IChatClient chatClient =
                new OpenAIClient(apiKey).
                    GetChatClient(model).
                    AsIChatClient();
           
            var historyProvider = new InMemoryChatHistoryProvider(
                new InMemoryChatHistoryProviderOptions
                {
                    ChatReducer = new MessageCountingChatReducer(20)
                });

            // Add the triage agent with tools
            services.AddSingleton<ServiceOpsAgentTeam>(serviceProvider =>
            {
                IServiceHealthReader healthReader = serviceProvider.GetRequiredService<IServiceHealthReader>();
                var healthTool = new ServiceHealthTool(healthReader);
                AIFunction getServiceHealth = AIFunctionFactory.Create(
                    healthTool.GetServiceHealthAsync,
                    new AIFunctionFactoryOptions
                    {
                        Name = "get_service_health",
                        Description = "Get the current read-only health status for an approved ServiceOps service."
                    });

                var searchOptions = new TextSearchProviderOptions
                {
                    SearchTime = TextSearchProviderOptions
                               .TextSearchBehavior
                               .BeforeAIInvoke,
                    RecentMessageMemoryLimit = 4
                };
                IOperationalKnowledgeSearch operationalKnowledgeSearch = serviceProvider.GetRequiredService<IOperationalKnowledgeSearch>();
                var knowledgeAdapter = new OperationalKnowledgeAdapter(operationalKnowledgeSearch);
                var knowledgeProvider = new TextSearchProvider
                (
                    knowledgeAdapter.SearchAsync,
                    searchOptions
                );

                ChatClientAgent knowledgeAgent = chatClient.AsAIAgent(
                    new ChatClientAgentOptions
                    {
                        Id = ServiceOpsAgentIds.Knowledge,
                        Name = "KnowledgeSpecialist",
                        Description = "Analyzes approved operational knowledge.",
                        ChatOptions = new()
                        {
                            Instructions = KnowledgeAgentInstructions.Text,
                        },
                        AIContextProviders = [knowledgeProvider]
                    });

                ChatClientAgent diagnosticsAgent = chatClient.AsAIAgent(
                    new ChatClientAgentOptions
                    {
                        Id = ServiceOpsAgentIds.Diagnostics,
                        Name = "DiagnosticsSpecialist",
                        Description = "Investigates live operational evidence.",
                        ChatOptions = new()
                        {
                            Instructions = DiagnosticsAgentInstructions.Text,
                            Tools = [getServiceHealth, 
                                    .. mcpTools]
                        }
                    });

                ChatClientAgent riskAgent = chatClient.AsAIAgent(
                    new ChatClientAgentOptions
                    {
                        Id = ServiceOpsAgentIds.Risk,
                        Name = "RiskSpecialist",
                        Description = "Review operational severity, escalation risk.",
                        ChatOptions = new()
                        {
                            Instructions = RiskAgentInstructions.Text
                        },
                        AIContextProviders = [knowledgeProvider]
                    });

                AIFunction knowledgeFunction = knowledgeAgent.AsAIFunction(
                    new AIFunctionFactoryOptions
                    {
                        Name = "consult_knowledge_specialist",
                        Description = "Consult approved operational runbooks and documentation."
                    });

                AIFunction diagnosticsFunction = diagnosticsAgent.AsAIFunction(
                    new AIFunctionFactoryOptions
                    {
                        Name = "consult_diagnostics_specialist",
                        Description = "Investigate current service, deployment and change evidence"
                    });

                AIFunction riskFunction = riskAgent.AsAIFunction(
                    new AIFunctionFactoryOptions
                    {
                        Name = "consult_risk_specialist",
                        Description = "Review severity, uncertainity, escalation and human-review risk."
                    });

                ChatClientAgent coordinator = chatClient.AsAIAgent(
                    new ChatClientAgentOptions
                    {
                        Id = ServiceOpsAgentIds.Coordinator,
                        Name = "ServiceOpsTriage",
                        Description = "Coordinates specialist analysis and produces the final triage assessment.",
                        ChatOptions = new()
                        {
                            Instructions = TriageAgentInstructions.Text,
                            Tools = [knowledgeFunction, diagnosticsFunction, riskFunction]
                        },
                        ChatHistoryProvider = historyProvider
                    });

                return new ServiceOpsAgentTeam(
                    coordinator,
                    knowledgeAgent,
                    diagnosticsAgent,
                    riskAgent);
            });
           
            services.AddSingleton<AIAgent>(
                serviceProvider => serviceProvider.GetRequiredService<ServiceOpsAgentTeam>().Coordinator);
            services.AddSingleton<IncidentTriageService>();
            return services;
        }
    }
}
