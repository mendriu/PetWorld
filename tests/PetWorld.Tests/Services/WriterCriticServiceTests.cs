using Moq;
using PetWorld.AI.Agents;
using PetWorld.AI.Services;
using OpenAI.Chat;

namespace PetWorld.Tests.Services;

public class WriterCriticServiceTests
{
    [Fact]
    public async Task GenerateResponseAsync_ApprovedOnFirstIteration_ReturnsAfterOneIteration()
    {
        // Arrange
        var writerAgentMock = new Mock<WriterAgent>(MockBehavior.Strict, (ChatClient)null!);
        var criticAgentMock = new Mock<CriticAgent>(MockBehavior.Strict, (ChatClient)null!);

        var question = "Jaka karma dla psa?";
        var response = "Polecam Royal Canin";

        writerAgentMock
            .Setup(x => x.GenerateResponseAsync(question, null))
            .ReturnsAsync(response);

        criticAgentMock
            .Setup(x => x.EvaluateResponseAsync(question, response))
            .ReturnsAsync((true, ""));

        var service = new WriterCriticService(writerAgentMock.Object, criticAgentMock.Object);

        // Act
        var (resultResponse, iterations) = await service.GenerateResponseAsync(question);

        // Assert
        Assert.Equal(response, resultResponse);
        Assert.Equal(1, iterations);
        writerAgentMock.Verify(x => x.GenerateResponseAsync(question, null), Times.Once);
    }

    [Fact]
    public async Task GenerateResponseAsync_ApprovedOnSecondIteration_ReturnsAfterTwoIterations()
    {
        // Arrange
        var writerAgentMock = new Mock<WriterAgent>(MockBehavior.Strict, (ChatClient)null!);
        var criticAgentMock = new Mock<CriticAgent>(MockBehavior.Strict, (ChatClient)null!);

        var question = "Jaka karma dla psa?";
        var firstResponse = "Karma";
        var secondResponse = "Polecam Royal Canin Adult Medium za 89.99 zl";
        var feedback = "Dodaj konkretne produkty i ceny";

        writerAgentMock
            .Setup(x => x.GenerateResponseAsync(question, null))
            .ReturnsAsync(firstResponse);

        writerAgentMock
            .Setup(x => x.GenerateResponseAsync(question, feedback))
            .ReturnsAsync(secondResponse);

        criticAgentMock
            .Setup(x => x.EvaluateResponseAsync(question, firstResponse))
            .ReturnsAsync((false, feedback));

        criticAgentMock
            .Setup(x => x.EvaluateResponseAsync(question, secondResponse))
            .ReturnsAsync((true, ""));

        var service = new WriterCriticService(writerAgentMock.Object, criticAgentMock.Object);

        // Act
        var (resultResponse, iterations) = await service.GenerateResponseAsync(question);

        // Assert
        Assert.Equal(secondResponse, resultResponse);
        Assert.Equal(2, iterations);
    }

    [Fact]
    public async Task GenerateResponseAsync_NeverApproved_ReturnsAfterMaxIterations()
    {
        // Arrange
        var writerAgentMock = new Mock<WriterAgent>(MockBehavior.Strict, (ChatClient)null!);
        var criticAgentMock = new Mock<CriticAgent>(MockBehavior.Strict, (ChatClient)null!);

        var question = "Test question";
        var feedback = "Needs improvement";

        writerAgentMock
            .Setup(x => x.GenerateResponseAsync(question, It.IsAny<string?>()))
            .ReturnsAsync("Response");

        criticAgentMock
            .Setup(x => x.EvaluateResponseAsync(question, "Response"))
            .ReturnsAsync((false, feedback));

        var service = new WriterCriticService(writerAgentMock.Object, criticAgentMock.Object);

        // Act
        var (resultResponse, iterations) = await service.GenerateResponseAsync(question);

        // Assert
        Assert.Equal("Response", resultResponse);
        Assert.Equal(3, iterations); // MaxIterations = 3
    }

    [Fact]
    public async Task GenerateResponseAsync_FeedbackPassedToWriter()
    {
        // Arrange
        var writerAgentMock = new Mock<WriterAgent>(MockBehavior.Strict, (ChatClient)null!);
        var criticAgentMock = new Mock<CriticAgent>(MockBehavior.Strict, (ChatClient)null!);

        var question = "Test question";
        var feedback = "Add more details";

        writerAgentMock
            .Setup(x => x.GenerateResponseAsync(question, null))
            .ReturnsAsync("First response");

        writerAgentMock
            .Setup(x => x.GenerateResponseAsync(question, feedback))
            .ReturnsAsync("Improved response");

        criticAgentMock
            .Setup(x => x.EvaluateResponseAsync(question, "First response"))
            .ReturnsAsync((false, feedback));

        criticAgentMock
            .Setup(x => x.EvaluateResponseAsync(question, "Improved response"))
            .ReturnsAsync((true, ""));

        var service = new WriterCriticService(writerAgentMock.Object, criticAgentMock.Object);

        // Act
        await service.GenerateResponseAsync(question);

        // Assert
        writerAgentMock.Verify(x => x.GenerateResponseAsync(question, feedback), Times.Once);
    }
}
