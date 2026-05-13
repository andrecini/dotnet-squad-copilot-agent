using Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Auth;

[ExcludeFromCodeCoverage]
public class ForgotPasswordEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task ForgotPasswordAsync_ExistingEmail_ReturnsOkAsync()
    {
        // Arrange
        var email = $"forgot_{Guid.NewGuid():N}@test.com";
        var registerRequest = RegisterUserRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var request = ForgotPasswordRequestMock.Valid(email);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPasswordAsync_UnknownEmail_ReturnsOkAsync()
    {
        // Arrange — por segurança (anti-enumeração), o endpoint retorna 200 mesmo quando o e-mail não existe
        var request = ForgotPasswordRequestMock.WithUnknownEmail();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPasswordAsync_InvalidEmail_ReturnsBadRequestAsync()
    {
        // Arrange
        var request = ForgotPasswordRequestMock.WithInvalidEmail();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
