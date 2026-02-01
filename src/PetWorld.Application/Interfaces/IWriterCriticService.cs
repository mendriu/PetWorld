namespace PetWorld.Application.Interfaces;

public interface IWriterCriticService
{
    Task<(string Response, int Iterations)> GenerateResponseAsync(string question);
}
