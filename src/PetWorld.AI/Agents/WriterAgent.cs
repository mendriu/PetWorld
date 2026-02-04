using OpenAI.Chat;

namespace PetWorld.AI.Agents;

public class WriterAgent
{
    private readonly ChatClient _chatClient;
    private const string ProductCatalog = @"
KATALOG PRODUKTÓW PETWORLD:

KARMA DLA PSÓW:
- Royal Canin Adult Medium - karma dla dorosłych psów średnich ras (89.99 zł/4kg)
- Purina Pro Plan Adult Large - karma dla dużych psów dorosłych (129.99 zł/7kg)
- Hill's Science Plan Puppy - karma dla szczeniąt (79.99 zł/2.5kg)
- Brit Premium by Nature Adult - karma naturalna dla dorosłych psów (59.99 zł/3kg)

KARMA DLA KOTÓW:
- Whiskas Adult Tuna - karma z tuńczykiem dla dorosłych kotów (45.99 zł/2kg)
- Royal Canin Indoor - karma dla kotów domowych (89.99 zł/4kg)
- Felix Adult Mixed - karma mokra mix smaków (49.99 zł/12 saszetek)
- Purina One Sterilised - karma dla kotów sterylizowanych (54.99 zł/1.5kg)

AKCESORIA DLA PSÓW:
- Smycz automatyczna Flexi 5m (79.99 zł)
- Obroża skórzana premium (49.99 zł)
- Legowisko ortopedyczne Memory Foam (199.99 zł)
- Zabawka Kong Classic (39.99 zł)

AKCESORIA DLA KOTÓW:
- Drapak wieżowy 120cm (149.99 zł)
- Kuweta samooczyszczająca (299.99 zł)
- Transporter plastikowy (89.99 zł)
- Zabawka laser pointer (19.99 zł)

KARMA DLA GRYZONI:
- Vitapol Premium dla chomików (24.99 zł/500g)
- Versele-Laga dla świnek morskich (29.99 zł/1kg)
- Beaphar Care+ dla królików (39.99 zł/1.5kg)

KARMA DLA PTAKÓW:
- Versele-Laga Prestige dla papug (34.99 zł/1kg)
- Vitapol dla kanarków (19.99 zł/500g)

KARMA DLA RYB:
- Tetra Min płatki (29.99 zł/100ml)
- Sera Vipan (24.99 zł/100ml)
";

    private const string SystemPrompt = @"Jesteś pomocnym asystentem sklepu PetWorld specjalizującego się w produktach dla zwierząt.
Twoim zadaniem jest:
1. Odpowiadać na pytania klientów o produkty dla zwierząt
2. Rekomendować odpowiednie produkty z naszego katalogu
3. Dawać porady dotyczące opieki nad zwierzętami
4. Być przyjaznym i profesjonalnym

Zawsze staraj się rekomendować konkretne produkty z katalogu wraz z cenami.
Odpowiadaj po polsku w przyjaznym, ale profesjonalnym tonie.

" + ProductCatalog;

    public WriterAgent(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public virtual async Task<string> GenerateResponseAsync(string question, string? feedback = null)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(SystemPrompt)
        };

        if (feedback != null)
        {
            messages.Add(new UserChatMessage($"Poprzednia odpowiedź wymagała poprawy. Feedback krytyka: {feedback}\n\nPytanie klienta: {question}\n\nProszę wygeneruj poprawioną odpowiedź uwzględniając feedback."));
        }
        else
        {
            messages.Add(new UserChatMessage(question));
        }

        var response = await _chatClient.CompleteChatAsync(messages);
        return response.Value.Content[0].Text;
    }
}
