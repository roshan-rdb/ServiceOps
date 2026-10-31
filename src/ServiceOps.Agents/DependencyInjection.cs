using Google.Protobuf.Reflection;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;
using ServiceOps.Agents.Knowledge;
using ServiceOps.Agents.Tools;
using ServiceOps.Agents.Triage;
using ServiceOps.Application.Knowledge;
using ServiceOps.Application.Operations;
using System.Xml.Linq;

namespace ServiceOps.Agents
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceOpsAgents(this IServiceCollection services, string model, string apiKey)
        {
            IChatClient chatClient =
                new OpenAIClient(apiKey).
                    GetChatClient(model).
                    AsIChatClient();

            //AIAgent triageAgent = chatClient.AsAIAgent(
            //    name: "ServiceOpsTriage",
            //    instructions: TriageAgentInstructions.Text);

            //services.AddSingleton(triageAgent);

            var historyProvider = new InMemoryChatHistoryProvider(
                new InMemoryChatHistoryProviderOptions
                {
                    ChatReducer = new MessageCountingChatReducer(20)
                });

            
           

            // Add the triage agent with tools
            services.AddSingleton<AIAgent>(serviceProvider =>
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

                return chatClient.AsAIAgent(
                    new ChatClientAgentOptions
                    {
                        Name = "ServiceOpsTriage",
                        ChatOptions = new()
                        {
                            Instructions = TriageAgentInstructions.Text,
                            Tools = [getServiceHealth]
                        },
                        ChatHistoryProvider = historyProvider,
                        AIContextProviders = [knowledgeProvider]
                    });
            });
            services.AddSingleton<IncidentTriageService>();
            return services;
        }
    }
}
