using System.IdentityModel.Tokens.Jwt;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Copilot.SquadAgent.StickerManager.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Security;

public class JwtTokenGeneratorTests
{
    private static IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "sticker-manager-test-secret-key-32chars!!",
                ["Jwt:Issuer"] = "StickerManagerApi",
                ["Jwt:Audience"] = "StickerManagerClient"
            })
            .Build();

    private static UserModel ValidUserModel() => new()
    {
        Id = Guid.NewGuid(),
        Email = "joao.silva@email.com",
        Name = "João Silva",
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public void Generate_ValidUser_ReturnsTokenWithBearerType()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var generator = new JwtTokenGenerator(configuration);
        var user = ValidUserModel();

        // Act
        var token = generator.Generate(user);

        // Assert
        token.ShouldNotBeNull();
        token.TokenType.ShouldBe("Bearer");
    }

    [Fact]
    public void Generate_ValidUser_ReturnsNonEmptyAccessToken()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var generator = new JwtTokenGenerator(configuration);
        var user = ValidUserModel();

        // Act
        var token = generator.Generate(user);

        // Assert
        token.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Generate_ValidUser_ReturnsExpiresIn3600()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var generator = new JwtTokenGenerator(configuration);
        var user = ValidUserModel();

        // Act
        var token = generator.Generate(user);

        // Assert
        token.ExpiresIn.ShouldBe(3600);
    }

    [Fact]
    public void Generate_ValidUser_TokenContainsUserIdClaim()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var generator = new JwtTokenGenerator(configuration);
        var user = ValidUserModel();

        // Act
        var token = generator.Generate(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.AccessToken);
        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        subClaim.ShouldNotBeNull();
        subClaim!.Value.ShouldBe(user.Id.ToString());
    }

    [Fact]
    public void Generate_ValidUser_TokenContainsEmailClaim()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var generator = new JwtTokenGenerator(configuration);
        var user = ValidUserModel();

        // Act
        var token = generator.Generate(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.AccessToken);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        emailClaim.ShouldNotBeNull();
        emailClaim!.Value.ShouldBe(user.Email);
    }

    [Fact]
    public void Generate_ValidUser_TokenIsNotExpired()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var generator = new JwtTokenGenerator(configuration);
        var user = ValidUserModel();

        // Act
        var token = generator.Generate(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.AccessToken);
        jwtToken.ValidTo.ShouldBeGreaterThan(DateTime.UtcNow);
    }
}
