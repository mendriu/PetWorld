using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;
using PetWorld.AI.Agents;
using PetWorld.AI.Services;
using PetWorld.Application.Interfaces;

namespace PetWorld.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAI(this IServiceCollection services, string? openAiApiKey)
    {
        if (!string.IsNullOrWhiteSpace(openAiApiKey))
        {
            services.AddSingleton(_ =>
            {
                var client = new OpenAIClient(openAiApiKey);
                return client.GetChatClient("gpt-4o");
            });

            services.AddScoped<WriterAgent>();
            services.AddScoped<CriticAgent>();
            services.AddScoped<IWriterCriticService, WriterCriticService>();
        }

        return services;
    }
}
