namespace PetWorld.Domain.Entities;

public class ChatHistory
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int IterationCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
