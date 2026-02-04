using PetWorld.Domain.Entities;

namespace PetWorld.Tests.Domain;

public class ChatHistoryTests
{
    [Fact]
    public void ChatHistory_DefaultValues_AreCorrect()
    {
        // Act
        var chatHistory = new ChatHistory();

        // Assert
        Assert.Equal(0, chatHistory.Id);
        Assert.Equal(string.Empty, chatHistory.Question);
        Assert.Equal(string.Empty, chatHistory.Answer);
        Assert.Equal(0, chatHistory.IterationCount);
        Assert.Equal(default(DateTime), chatHistory.CreatedAt);
    }

    [Fact]
    public void ChatHistory_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var chatHistory = new ChatHistory
        {
            Id = 1,
            Question = "Test question",
            Answer = "Test answer",
            IterationCount = 2,
            CreatedAt = now
        };

        // Assert
        Assert.Equal(1, chatHistory.Id);
        Assert.Equal("Test question", chatHistory.Question);
        Assert.Equal("Test answer", chatHistory.Answer);
        Assert.Equal(2, chatHistory.IterationCount);
        Assert.Equal(now, chatHistory.CreatedAt);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ChatHistory_IterationCount_AcceptsValidValues(int iterations)
    {
        // Act
        var chatHistory = new ChatHistory { IterationCount = iterations };

        // Assert
        Assert.Equal(iterations, chatHistory.IterationCount);
    }

    [Fact]
    public void ChatHistory_Question_CanContainPolishCharacters()
    {
        // Arrange
        var question = "Jaka karma dla psa jest najlepsza? Mam owczarka niemieckiego.";

        // Act
        var chatHistory = new ChatHistory { Question = question };

        // Assert
        Assert.Equal(question, chatHistory.Question);
        Assert.Contains("owczarka", chatHistory.Question);
    }

    [Fact]
    public void ChatHistory_Answer_CanContainSpecialCharacters()
    {
        // Arrange
        var answer = "Royal Canin Adult - 89.99 zł/4kg\nPurina Pro Plan - 129.99 zł/7kg";

        // Act
        var chatHistory = new ChatHistory { Answer = answer };

        // Assert
        Assert.Equal(answer, chatHistory.Answer);
        Assert.Contains("zł", chatHistory.Answer);
        Assert.Contains("\n", chatHistory.Answer);
    }
}
