using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;
using ServiceOps.Agents.Tools;
using ServiceOps.Agents.Triage;
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

                return new ChatClientAgent(
                     chatClient,
                     name: "ServiceOpsTriage",
                     instructions: TriageAgentInstructions.Text,
                     tools: [getServiceHealth]);
            });
            services.AddSingleton<IncidentTriageService>();
            return services;
        }
    }
}
