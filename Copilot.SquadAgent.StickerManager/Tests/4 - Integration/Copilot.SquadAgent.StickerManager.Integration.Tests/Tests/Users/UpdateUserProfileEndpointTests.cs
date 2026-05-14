using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Helpers;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Users;

[ExcludeFromCodeCoverage]
public class UpdateUserProfileEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task UpdateProfileAsync_ValidName_ReturnsOkWithUpdatedProfileAsync()
    {
        // Arrange
        var email = $"upd_name_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = UpdateUserProfileRequestMock.Valid();

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/profile", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        body.ShouldNotBeNull();
        body.Name.ShouldBe("Updated Name");
        body.UserId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task UpdateProfileAsync_ValidEmail_ReturnsOkWithUpdatedEmailAsync()
    {
        // Arrange
        var originalEmail = $"upd_email_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, originalEmail);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var newEmail = $"upd_new_{Guid.NewGuid():N}@test.com";
        var request = UpdateUserProfileRequestMock.WithNewEmail(newEmail);

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/profile", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        body.ShouldNotBeNull();
        body.Email.ShouldBe(newEmail);
    }

    [Fact]
    public async Task UpdateProfileAsync_DuplicateEmail_ReturnsProblemAsync()
    {
        // Arrange — registra dois usuários distintos; tenta atualizar o segundo com o e-mail do primeiro
        var firstEmail = $"dup_first_{Guid.NewGuid():N}@test.com";
        await AuthHelper.GetTokenAsync(_client, firstEmail);

        var secondEmail = $"dup_second_{Guid.NewGuid():N}@test.com";
        var tokenSecond = await AuthHelper.GetTokenAsync(_client, secondEmail);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenSecond);

        var request = UpdateUserProfileRequestMock.WithNewEmail(firstEmail);

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/profile", request);

        // Assert — a API deve rejeitar a atualização para um e-mail já cadastrado (4xx)
        ((int)response.StatusCode).ShouldBeGreaterThanOrEqualTo(400);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        var request = UpdateUserProfileRequestMock.Valid();

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/profile", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
