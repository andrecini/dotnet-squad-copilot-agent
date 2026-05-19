using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Auth;

[ExcludeFromCodeCoverage]
public class ResetPasswordEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_ReturnsOkAsync()
    {
        // Arrange
        var email = $"reset_{Guid.NewGuid():N}@test.com";
        var registerRequest = RegisterUserRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var forgotRequest = ForgotPasswordRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", forgotRequest);

        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenEntity = await db.PasswordResetTokens
            .Where(t => t.User.Email == email && !t.IsUsed)
            .FirstAsync();

        var request = ResetPasswordRequestMock.Valid(tokenEntity.Token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPasswordAsync_InvalidToken_ReturnsProblemAsync()
    {
        // Arrange
        var request = ResetPasswordRequestMock.WithInvalidToken();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        // Assert
        ((int)response.StatusCode).ShouldBeGreaterThanOrEqualTo(400);
    }

    [Fact]
    public async Task ResetPasswordAsync_ExpiredToken_ReturnsProblemAsync()
    {
        // Arrange
        var email = $"expired_{Guid.NewGuid():N}@test.com";
        var registerRequest = RegisterUserRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.FirstAsync(u => u.Email == email);

        var expiredToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = "expired-token-test",
            ExpiresAt = DateTime.UtcNow.AddHours(-1),
            IsUsed = false
        };
        db.PasswordResetTokens.Add(expiredToken);
        await db.SaveChangesAsync();

        var request = ResetPasswordRequestMock.Valid("expired-token-test");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        // Assert
        ((int)response.StatusCode).ShouldBeGreaterThanOrEqualTo(400);
    }

    [Fact]
    public async Task ResetPasswordAsync_WeakNewPassword_ReturnsBadRequestAsync()
    {
        // Arrange
        var email = $"weakpwd_{Guid.NewGuid():N}@test.com";
        var registerRequest = RegisterUserRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var forgotRequest = ForgotPasswordRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", forgotRequest);

        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenEntity = await db.PasswordResetTokens
            .Where(t => t.User.Email == email && !t.IsUsed)
            .FirstAsync();

        var request = ResetPasswordRequestMock.WithWeakNewPassword(tokenEntity.Token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
