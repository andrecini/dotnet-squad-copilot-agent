using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Helpers;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Users;

public class GetUserProfileEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task GetProfileAsync_AuthenticatedUser_ReturnsOkWithProfileAsync()
    {
        // Arrange
        var email = $"getprofile_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        body.ShouldNotBeNull();
        body.UserId.ShouldNotBe(Guid.Empty);
        body.Email.ShouldBe(email);
    }

    [Fact]
    public async Task GetProfileAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfileAsync_WithInvalidToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token-invalido");

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
