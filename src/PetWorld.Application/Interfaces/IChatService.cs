using PetWorld.Application.DTOs;

namespace PetWorld.Application.Interfaces;

public interface IChatService
{
    Task<ChatResponseDto> ProcessQuestionAsync(string question);
}
