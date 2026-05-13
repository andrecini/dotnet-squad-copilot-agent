using System.Net;
using System.Net.Http.Json;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Auth;

public class LoginUserEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsOkWithTokenAsync()
    {
        // Arrange
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var registerRequest = RegisterUserRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = LoginUserRequestMock.Valid(email);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        body.ShouldNotBeNull();
        body.AccessToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsProblemAsync()
    {
        // Arrange
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var registerRequest = RegisterUserRequestMock.Valid(email);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = LoginUserRequestMock.WithWrongPassword(email);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        ((int)response.StatusCode).ShouldBeGreaterThanOrEqualTo(400);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsProblemAsync()
    {
        // Arrange
        var loginRequest = LoginUserRequestMock.Valid($"unknown_{Guid.NewGuid():N}@test.com");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        ((int)response.StatusCode).ShouldBeGreaterThanOrEqualTo(400);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmailFormat_ReturnsBadRequestAsync()
    {
        // Arrange
        var loginRequest = LoginUserRequestMock.WithInvalidEmail();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
