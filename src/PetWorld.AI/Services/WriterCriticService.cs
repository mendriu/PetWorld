using PetWorld.AI.Agents;
using PetWorld.Application.Interfaces;

namespace PetWorld.AI.Services;

public class WriterCriticService : IWriterCriticService
{
    private readonly WriterAgent _writerAgent;
    private readonly CriticAgent _criticAgent;
    private const int MaxIterations = 3;

    public WriterCriticService(WriterAgent writerAgent, CriticAgent criticAgent)
    {
        _writerAgent = writerAgent;
        _criticAgent = criticAgent;
    }

    public async Task<(string Response, int Iterations)> GenerateResponseAsync(string question)
    {
        string? feedback = null;
        string response = string.Empty;
        int iteration = 0;

        while (iteration < MaxIterations)
        {
            iteration++;

            response = await _writerAgent.GenerateResponseAsync(question, feedback);

            var (approved, criticFeedback) = await _criticAgent.EvaluateResponseAsync(question, response);

            if (approved)
            {
                break;
            }

            if (iteration < MaxIterations)
            {
                feedback = criticFeedback;
            }
        }

        return (response, iteration);
    }
}
