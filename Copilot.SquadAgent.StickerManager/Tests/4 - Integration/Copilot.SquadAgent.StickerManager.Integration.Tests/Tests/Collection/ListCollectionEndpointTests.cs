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
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Collection;

[ExcludeFromCodeCoverage]
public class ListCollectionEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

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

    private async Task AddStickerToCollectionAsync(HttpClient client, Guid stickerId)
    {
        await client.PostAsJsonAsync("/api/v1/collection", new { sticker_id = stickerId });
    }

    [Fact]
    public async Task ListCollectionAsync_WithStickers_ReturnsOkWithItemsAsync()
    {
        // Arrange
        var (stickerId, _) = await SeedStickerAsync();

        var email = $"list_ok_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await AddStickerToCollectionAsync(_client, stickerId);

        // Act
        var response = await _client.GetAsync("/api/v1/collection?page=1&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();
        body.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ListCollectionAsync_WithoutStickers_ReturnsOkWithEmptyListAsync()
    {
        // Arrange
        var email = $"list_empty_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection?page=1&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();
        body.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListCollectionAsync_WithTeamFilter_ReturnsFilteredResultsAsync()
    {
        // Arrange
        var (stickerId, teamId) = await SeedStickerAsync("Vinicius Jr");

        var email = $"list_team_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await AddStickerToCollectionAsync(_client, stickerId);

        // Act
        var response = await _client.GetAsync($"/api/v1/collection?TeamId={teamId}&page=1&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();
        body.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ListCollectionAsync_WithRarityFilter_ReturnsFilteredResultsAsync()
    {
        // Arrange
        var (stickerId, _) = await SeedStickerAsync("Rodrygo", StickerRarity.Foil);

        var email = $"list_rarity_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await AddStickerToCollectionAsync(_client, stickerId);

        // Act
        var response = await _client.GetAsync("/api/v1/collection?Rarity=Foil&page=1&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();
        body.ShouldAllBe(item => item.Rarity == StickerRarity.Foil);
    }

    [Fact]
    public async Task ListCollectionAsync_WithSortByPlayerName_ReturnsOkAsync()
    {
        // Arrange
        var (stickerId, _) = await SeedStickerAsync("Alisson");

        var email = $"list_sort_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await AddStickerToCollectionAsync(_client, stickerId);

        // Act
        var response = await _client.GetAsync("/api/v1/collection?Sort=player_name&page=1&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();
    }

    [Fact]
    public async Task ListCollectionAsync_WithPagination_ReturnsPaginatedResultsAsync()
    {
        // Arrange
        var (stickerId, _) = await SeedStickerAsync("Marquinhos");

        var email = $"list_page_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await AddStickerToCollectionAsync(_client, stickerId);

        // Act — página 1 com limit 1 deve retornar no máximo 1 item
        var response = await _client.GetAsync("/api/v1/collection?page=1&limit=1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();
        body.Count.ShouldBeLessThanOrEqualTo(1);
    }

    [Fact]
    public async Task ListCollectionAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/collection");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListCollectionAsync_InvalidPage_ReturnsBadRequestAsync()
    {
        // Arrange
        var email = $"list_invalid_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection?page=0&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListCollectionAsync_ResponseContainsExpectedFields_ReturnsOkAsync()
    {
        // Arrange
        var (stickerId, _) = await SeedStickerAsync("Casemiro");

        var email = $"list_fields_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await AddStickerToCollectionAsync(_client, stickerId);

        // Act
        var response = await _client.GetAsync("/api/v1/collection?page=1&limit=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<CollectionItemResponse>>(JsonOptions);
        body.ShouldNotBeNull();

        var item = body.FirstOrDefault(x => x.StickerId == stickerId);
        item.ShouldNotBeNull();
        item!.PlayerName.ShouldBe("Casemiro");
        item.Team.ShouldBe("Brasil");
        item.QuantityOwned.ShouldBeGreaterThan(0);
        item.StickerId.ShouldNotBe(Guid.Empty);
    }
}
