using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Data;

namespace PetWorld.Infrastructure.Repositories;

public class ChatHistoryRepository : IChatHistoryRepository
{
    private readonly PetWorldDbContext _context;

    public ChatHistoryRepository(PetWorldDbContext context)
    {
        _context = context;
    }

    public async Task<ChatHistory> AddAsync(ChatHistory chatHistory)
    {
        _context.ChatHistories.Add(chatHistory);
        await _context.SaveChangesAsync();
        return chatHistory;
    }

    public async Task<IEnumerable<ChatHistory>> GetAllAsync()
    {
        return await _context.ChatHistories
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}
