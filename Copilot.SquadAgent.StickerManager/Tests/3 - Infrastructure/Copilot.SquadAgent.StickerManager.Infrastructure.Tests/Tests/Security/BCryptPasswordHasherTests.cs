using Copilot.SquadAgent.StickerManager.Infrastructure.Security;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Security;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_ValidPassword_ReturnsDifferentValueThanInput()
    {
        // Arrange
        var password = "senha123";

        // Act
        var hash = _hasher.Hash(password);

        // Assert
        hash.ShouldNotBe(password);
    }

    [Fact]
    public void Hash_SamePasswordCalledTwice_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "senha123";

        // Act
        var hash1 = _hasher.Hash(password);
        var hash2 = _hasher.Hash(password);

        // Assert
        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "senha123";
        var hash = _hasher.Hash(password);

        // Act
        var result = _hasher.Verify(password, hash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        // Arrange
        var hash = _hasher.Hash("senha123");

        // Act
        var result = _hasher.Verify("senhaErrada", hash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Verify_EmptyPassword_ReturnsFalse()
    {
        // Arrange
        var hash = _hasher.Hash("senha123");

        // Act
        var result = _hasher.Verify(string.Empty, hash);

        // Assert
        result.ShouldBeFalse();
    }
}
