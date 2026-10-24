using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using ServiceOps.Agents.Triage;

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

            AIAgent triageAgent = chatClient.AsAIAgent(
                name: "ServiceOpsTriage",
                instructions: TriageAgentInstructions.Text);

            services.AddSingleton(triageAgent);
            services.AddSingleton<IncidentTriageService>();

            return services;
        }
    }
}
