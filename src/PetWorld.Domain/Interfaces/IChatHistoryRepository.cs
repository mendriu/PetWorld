using PetWorld.Domain.Entities;

namespace PetWorld.Domain.Interfaces;

public interface IChatHistoryRepository
{
    Task<ChatHistory> AddAsync(ChatHistory chatHistory);
    Task<IEnumerable<ChatHistory>> GetAllAsync();
}
