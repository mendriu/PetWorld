using Moq;
using PetWorld.Application.Interfaces;
using PetWorld.Application.Services;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Interfaces;

namespace PetWorld.Tests.Services;

public class ChatServiceTests
{
    private readonly Mock<IWriterCriticService> _writerCriticServiceMock;
    private readonly Mock<IChatHistoryRepository> _chatHistoryRepositoryMock;
    private readonly ChatService _chatService;

    public ChatServiceTests()
    {
        _writerCriticServiceMock = new Mock<IWriterCriticService>();
        _chatHistoryRepositoryMock = new Mock<IChatHistoryRepository>();
        _chatService = new ChatService(_writerCriticServiceMock.Object, _chatHistoryRepositoryMock.Object);
    }

    [Fact]
    public async Task ProcessQuestionAsync_ReturnsCorrectResponse()
    {
        // Arrange
        var question = "Jaka karma dla psa?";
        var expectedAnswer = "Polecam Royal Canin Adult Medium";
        var expectedIterations = 2;

        _writerCriticServiceMock
            .Setup(x => x.GenerateResponseAsync(question))
            .ReturnsAsync((expectedAnswer, expectedIterations));

        _chatHistoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatHistory>()))
            .ReturnsAsync((ChatHistory h) => h);

        // Act
        var result = await _chatService.ProcessQuestionAsync(question);

        // Assert
        Assert.Equal(expectedAnswer, result.Answer);
        Assert.Equal(expectedIterations, result.IterationCount);
    }

    [Fact]
    public async Task ProcessQuestionAsync_SavesChatHistory()
    {
        // Arrange
        var question = "Jaka karma dla kota?";
        var answer = "Polecam Whiskas Adult";
        var iterations = 1;

        _writerCriticServiceMock
            .Setup(x => x.GenerateResponseAsync(question))
            .ReturnsAsync((answer, iterations));

        ChatHistory? savedHistory = null;
        _chatHistoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatHistory>()))
            .Callback<ChatHistory>(h => savedHistory = h)
            .ReturnsAsync((ChatHistory h) => h);

        // Act
        await _chatService.ProcessQuestionAsync(question);

        // Assert
        Assert.NotNull(savedHistory);
        Assert.Equal(question, savedHistory.Question);
        Assert.Equal(answer, savedHistory.Answer);
        Assert.Equal(iterations, savedHistory.IterationCount);
    }

    [Fact]
    public async Task ProcessQuestionAsync_CallsWriterCriticService()
    {
        // Arrange
        var question = "Test question";
        _writerCriticServiceMock
            .Setup(x => x.GenerateResponseAsync(question))
            .ReturnsAsync(("response", 1));

        _chatHistoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatHistory>()))
            .ReturnsAsync((ChatHistory h) => h);

        // Act
        await _chatService.ProcessQuestionAsync(question);

        // Assert
        _writerCriticServiceMock.Verify(x => x.GenerateResponseAsync(question), Times.Once);
    }

    [Fact]
    public async Task ProcessQuestionAsync_CallsRepositoryAddAsync()
    {
        // Arrange
        var question = "Test question";
        _writerCriticServiceMock
            .Setup(x => x.GenerateResponseAsync(question))
            .ReturnsAsync(("response", 1));

        _chatHistoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatHistory>()))
            .ReturnsAsync((ChatHistory h) => h);

        // Act
        await _chatService.ProcessQuestionAsync(question);

        // Assert
        _chatHistoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ChatHistory>()), Times.Once);
    }

    [Theory]
    [InlineData("", "Empty response", 1)]
    [InlineData("Question", "", 1)]
    [InlineData("Question", "Response", 3)]
    public async Task ProcessQuestionAsync_HandlesVariousInputs(string question, string answer, int iterations)
    {
        // Arrange
        _writerCriticServiceMock
            .Setup(x => x.GenerateResponseAsync(question))
            .ReturnsAsync((answer, iterations));

        _chatHistoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatHistory>()))
            .ReturnsAsync((ChatHistory h) => h);

        // Act
        var result = await _chatService.ProcessQuestionAsync(question);

        // Assert
        Assert.Equal(answer, result.Answer);
        Assert.Equal(iterations, result.IterationCount);
    }
}
