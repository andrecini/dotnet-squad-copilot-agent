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
public class GetAlbumEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
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

    [Fact]
    public async Task GetAlbumAsync_AuthenticatedUser_ReturnsOkAsync()
    {
        // Arrange
        await SeedStickerAsync("Vinicius Jr");

        var email = $"album_ok_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/album");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedAlbumResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAlbumAsync_WithPagination_ReturnsCorrectPageAsync()
    {
        // Arrange
        await SeedStickerAsync("Casemiro");

        var email = $"album_page_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/album?page=1&pageSize=5");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedAlbumResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Page.ShouldBe(1);
        body.PageSize.ShouldBe(5);
        body.Items.Count.ShouldBeLessThanOrEqualTo(5);
        body.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        body.TotalPages.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetAlbumAsync_WithSortByTeam_ReturnsOkAsync()
    {
        // Arrange
        await SeedStickerAsync("Marquinhos");

        var email = $"album_sort_team_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/album?sortByTeam=true");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedAlbumResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAlbumAsync_WithTeamIdFilter_ReturnsFilteredResultsAsync()
    {
        // Arrange
        var (stickerId, teamId) = await SeedStickerAsync("Alisson");

        var email = $"album_filter_team_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/v1/album?teamId={teamId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedAlbumResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
        body.Items.ShouldAllBe(item => item.Team == "Brasil");
    }

    [Fact]
    public async Task GetAlbumAsync_ResponseContainsPaginationFields_ReturnsOkAsync()
    {
        // Arrange
        await SeedStickerAsync("Rodrygo");

        var email = $"album_fields_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/album?page=1&pageSize=20");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedAlbumResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
        body.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        body.Page.ShouldBe(1);
        body.PageSize.ShouldBe(20);
        body.TotalPages.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetAlbumAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/album");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
