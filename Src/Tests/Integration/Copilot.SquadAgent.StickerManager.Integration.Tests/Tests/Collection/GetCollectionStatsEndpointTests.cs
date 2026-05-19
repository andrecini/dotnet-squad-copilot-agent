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
public class GetCollectionStatsEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
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
    public async Task GetCollectionStatsAsync_AuthenticatedUserWithStickers_ReturnsOkWithStatsAsync()
    {
        // Arrange
        var (stickerId, _) = await SeedStickerAsync("Vinicius Jr");

        var email = $"stats_ok_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/v1/collection", new { sticker_id = stickerId });

        // Act
        var response = await _client.GetAsync("/api/v1/collection/stats");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CollectionStatsResponse>();
        body.ShouldNotBeNull();
        body.TotalOwned.ShouldBeGreaterThan(0);
        body.TotalStickers.ShouldBeGreaterThanOrEqualTo(0);
        body.CompletionPercentage.ShouldBeGreaterThanOrEqualTo(0);
        body.Duplicates.ShouldBeGreaterThanOrEqualTo(0);
        body.TotalMissing.ShouldBeGreaterThanOrEqualTo(0);
        body.ByTeam.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetCollectionStatsAsync_AuthenticatedUserWithNoStickers_ReturnsOkWithZeroStatsAsync()
    {
        // Arrange
        var email = $"stats_empty_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/stats");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CollectionStatsResponse>();
        body.ShouldNotBeNull();
        body.TotalOwned.ShouldBe(0);
        body.CompletionPercentage.ShouldBe(0);
        body.Duplicates.ShouldBe(0);
        body.ByTeam.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetCollectionStatsAsync_ResponseContainsExpectedFields_ReturnsOkAsync()
    {
        // Arrange
        var (stickerId, teamId) = await SeedStickerAsync("Casemiro");

        var email = $"stats_fields_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/v1/collection", new { sticker_id = stickerId });

        // Act
        var response = await _client.GetAsync("/api/v1/collection/stats");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CollectionStatsResponse>();
        body.ShouldNotBeNull();
        body.TotalOwned.ShouldBeGreaterThanOrEqualTo(0);
        body.TotalMissing.ShouldBeGreaterThanOrEqualTo(0);
        body.TotalStickers.ShouldBeGreaterThanOrEqualTo(0);
        body.CompletionPercentage.ShouldBeInRange(0, 100);
        body.Duplicates.ShouldBeGreaterThanOrEqualTo(0);
        body.ByTeam.ShouldNotBeNull();

        var teamStats = body.ByTeam.FirstOrDefault(x => x.TeamId == teamId);
        teamStats.ShouldNotBeNull();
        teamStats!.Team.ShouldBe("Brasil");
        teamStats.TeamCode.ShouldBe("BRA");
        teamStats.Owned.ShouldBeGreaterThan(0);
        teamStats.Total.ShouldBeGreaterThan(0);
        teamStats.CompletionPercentage.ShouldBeInRange(0, 100);
    }

    [Fact]
    public async Task GetCollectionStatsAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/collection/stats");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
