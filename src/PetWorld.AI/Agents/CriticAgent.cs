using System.Text.Json;
using OpenAI.Chat;

namespace PetWorld.AI.Agents;

public class CriticAgent
{
    private readonly ChatClient _chatClient;

    private const string SystemPrompt = @"Jesteś krytykiem odpowiedzi asystenta sklepu PetWorld.
Twoje zadanie to ocenić odpowiedź pod kątem:
1. Poprawności informacji o produktach
2. Profesjonalnego i przyjaznego tonu
3. Kompletności odpowiedzi (czy odpowiedź adresuje pytanie klienta)
4. Czy zaproponowano konkretne produkty z katalogu (jeśli to stosowne)

Odpowiedz TYLKO w formacie JSON:
{
    ""approved"": true/false,
    ""feedback"": ""Twój feedback tutaj (jeśli approved=false)""
}

Jeśli odpowiedź jest akceptowalna, ustaw approved=true i feedback może być pusty.
Jeśli odpowiedź wymaga poprawy, ustaw approved=false i podaj konkretny feedback co należy poprawić.";

    public CriticAgent(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public virtual async Task<(bool Approved, string Feedback)> EvaluateResponseAsync(string question, string response)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage($"Pytanie klienta: {question}\n\nOdpowiedź asystenta: {response}")
        };

        var chatResponse = await _chatClient.CompleteChatAsync(messages);
        var content = chatResponse.Value.Content[0].Text;

        try
        {
            var jsonStart = content.IndexOf('{');
            var jsonEnd = content.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                content = content.Substring(jsonStart, jsonEnd - jsonStart + 1);
            }

            var result = JsonSerializer.Deserialize<CriticResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return (result?.Approved ?? true, result?.Feedback ?? string.Empty);
        }
        catch
        {
            return (true, string.Empty);
        }
    }

    private class CriticResponse
    {
        public bool Approved { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
