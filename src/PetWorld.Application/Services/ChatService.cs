using PetWorld.Application.DTOs;
using PetWorld.Application.Interfaces;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Application.Services;

public class ChatService : IChatService
{
    private readonly IWriterCriticService _writerCriticService;
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public ChatService(IWriterCriticService writerCriticService, IChatHistoryRepository chatHistoryRepository)
    {
        _writerCriticService = writerCriticService;
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<ChatResponseDto> ProcessQuestionAsync(string question)
    {
        var (response, iterations) = await _writerCriticService.GenerateResponseAsync(question);

        var chatHistory = new ChatHistory
        {
            Question = question,
            Answer = response,
            IterationCount = iterations,
            CreatedAt = DateTime.UtcNow
        };

        await _chatHistoryRepository.AddAsync(chatHistory);

        return new ChatResponseDto
        {
            Answer = response,
            IterationCount = iterations
        };
    }
}
