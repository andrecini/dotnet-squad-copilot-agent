using System.Net;
using System.Net.Http.Json;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Auth;

public class RegisterUserEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsCreatedAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.Valid();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        body.ShouldNotBeNull();
        body.UserId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsProblemAsync()
    {
        // Arrange
        var email = $"dup_{Guid.NewGuid():N}@test.com";
        var firstRequest = RegisterUserRequestMock.Valid(email);
        var duplicateRequest = RegisterUserRequestMock.WithDuplicateEmail(email);

        await _client.PostAsJsonAsync("/api/v1/auth/register", firstRequest);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", duplicateRequest);

        // Assert
        ((int)response.StatusCode).ShouldBeGreaterThanOrEqualTo(400);
    }

    [Fact]
    public async Task RegisterAsync_InvalidEmail_ReturnsBadRequestAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithInvalidEmail();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterAsync_WeakPassword_ReturnsBadRequestAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithWeakPassword();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterAsync_EmptyName_ReturnsBadRequestAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithEmptyName();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
