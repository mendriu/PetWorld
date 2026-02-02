using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;
using PetWorld.AI.Agents;
using PetWorld.AI.Services;
using PetWorld.Application.Interfaces;

namespace PetWorld.AI;

public static class DependencyInjection
{
    /// <summary>
    /// Konfiguruje serwisy AI w zaleznosci od srodowiska:
    /// - Development: uzywa Ollama (localhost:11434)
    /// - Production: wymaga klucza OpenAI API
    /// </summary>
    public static IServiceCollection AddAI(
        this IServiceCollection services,
        string? openAiApiKey,
        bool isDevelopment,
        string ollamaEndpoint = "http://localhost:11434/v1/",
        string ollamaModel = "llama3.2:latest")
    {
        if (isDevelopment)
        {
            // DEVELOPMENT: Ollama (lokalne LLM)
            Console.WriteLine("[AI] Tryb DEVELOPMENT - uzywam Ollama ({0})", ollamaModel);

            services.AddSingleton(_ =>
            {
                var client = new OpenAIClient(
                    new System.ClientModel.ApiKeyCredential("ollama"), // Ollama nie wymaga klucza
                    new OpenAIClientOptions
                    {
                        Endpoint = new Uri(ollamaEndpoint)
                    });
                return client.GetChatClient(ollamaModel);
            });

            // Rejestruj agentow i serwisy
            services.AddScoped<WriterAgent>();
            services.AddScoped<CriticAgent>();
            services.AddScoped<IWriterCriticService, WriterCriticService>();
        }
        else
        {
            // PRODUCTION: OpenAI (wymaga klucza API)
            if (string.IsNullOrWhiteSpace(openAiApiKey))
            {
                Console.WriteLine("[AI] UWAGA: Tryb PRODUCTION bez klucza OpenAI - AI niedostepne!");
                return services;
            }

            Console.WriteLine("[AI] Tryb PRODUCTION - uzywam OpenAI (gpt-4o)");

            services.AddSingleton(_ =>
            {
                var client = new OpenAIClient(openAiApiKey);
                return client.GetChatClient("gpt-4o");
            });

            // Rejestruj agentow i serwisy
            services.AddScoped<WriterAgent>();
            services.AddScoped<CriticAgent>();
            services.AddScoped<IWriterCriticService, WriterCriticService>();
        }

        return services;
    }
}
