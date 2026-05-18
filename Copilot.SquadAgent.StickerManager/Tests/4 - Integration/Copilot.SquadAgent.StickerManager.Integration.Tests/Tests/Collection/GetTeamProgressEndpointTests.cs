using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Collection;

[ExcludeFromCodeCoverage]
public class GetTeamProgressEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    private async Task<(Guid stickerId, Guid teamId)> SeedStickerAsync(string playerName = "Neymar", StickerRarity rarity = StickerRarity.Base)
    {
        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var team = new Team
        {
            Name = "Brasil",
            Code = "BRA",
            FlagUrl = "https://example.com/flag.png",
            CreatedAt = DateTime.UtcNow
        };

        db.Teams.Add(team);

        var sticker = new Sticker
        {
            Code = $"BRA-{Guid.NewGuid():N}".Substring(0, 10),
            PlayerName = playerName,
            Rarity = rarity,
            TeamId = team.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.Stickers.Add(sticker);
        await db.SaveChangesAsync();

        return (sticker.Id, team.Id);
    }

    [Fact]
    public async Task GetTeamProgressAsync_ValidTeamIdAndAuthenticatedUser_ReturnsOkAsync()
    {
        // Arrange
        var (_, teamId) = await SeedStickerAsync("Vinicius Jr");

        var email = $"progress_ok_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/album/{teamId}/progress");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TeamProgressResponse>();
        body.ShouldNotBeNull();
        body.TeamId.ShouldBe(teamId);
        body.TeamName.ShouldBe("Brasil");
        body.TotalStickers.ShouldBeGreaterThan(0);
        body.OwnedCount.ShouldBeGreaterThanOrEqualTo(0);
        body.CompletionPercentage.ShouldBeInRange(0, 100);
    }

    [Fact]
    public async Task GetTeamProgressAsync_ResponseContainsExpectedFields_ReturnsOkAsync()
    {
        // Arrange
        var (stickerId, teamId) = await SeedStickerAsync("Casemiro");

        var email = $"progress_fields_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/v1/collection", new { sticker_id = stickerId });

        // Act
        var response = await _client.GetAsync($"/api/v1/album/{teamId}/progress");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TeamProgressResponse>();
        body.ShouldNotBeNull();
        body.TeamId.ShouldNotBe(Guid.Empty);
        body.TeamName.ShouldNotBeNullOrEmpty();
        body.TotalStickers.ShouldBeGreaterThan(0);
        body.OwnedCount.ShouldBeGreaterThan(0);
        body.CompletionPercentage.ShouldBeInRange(0, 100);
    }

    [Fact]
    public async Task GetTeamProgressAsync_NonExistentTeamId_ReturnsNotFoundAsync()
    {
        // Arrange
        var email = $"progress_notfound_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var nonExistentTeamId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/album/{nonExistentTeamId}/progress");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTeamProgressAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var teamId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/album/{teamId}/progress");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
