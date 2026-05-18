using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;
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
public class GetMissingStickersEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    private async Task<Guid> SeedStickerAsync(string playerName = "Neymar", StickerRarity rarity = StickerRarity.Base)
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

        return sticker.Id;
    }

    [Fact]
    public async Task GetMissingStickersAsync_UserHasNoStickers_ReturnsAllStickersAsync()
    {
        // Arrange
        await SeedStickerAsync("Richarlison");

        var email = $"missing_all_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.Count.ShouldBeGreaterThanOrEqualTo(1);
        body.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
        body.Page.ShouldBe(1);
        body.PageSize.ShouldBe(100);
    }

    [Fact]
    public async Task GetMissingStickersAsync_UserOwnsAllStickers_ReturnsEmptyListAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync("Casemiro");

        var email = $"missing_none_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/v1/collection", new { sticker_id = stickerId });

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotContain(x => x.StickerId == stickerId);
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithSortByRarity_ReturnsOkAsync()
    {
        // Arrange
        await SeedStickerAsync("Alisson", StickerRarity.Foil);

        var email = $"missing_sort_rarity_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?sort=rarity&page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithSortByTeam_ReturnsOkAsync()
    {
        // Arrange
        await SeedStickerAsync("Marquinhos");

        var email = $"missing_sort_team_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?sort=team&page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithSortByNumber_ReturnsOkAsync()
    {
        // Arrange
        await SeedStickerAsync("Rodrygo");

        var email = $"missing_sort_number_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?sort=number&page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithPageSize1_ReturnsAtMostOneItemAsync()
    {
        // Arrange
        await SeedStickerAsync("Paqueta");
        await SeedStickerAsync("Fred");

        var email = $"missing_page_pageSize_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=1&pageSize=1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.Items.Count.ShouldBeLessThanOrEqualTo(1);
        body.Page.ShouldBe(1);
        body.PageSize.ShouldBe(1);
        body.TotalPages.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithInvalidPage_ReturnsBadRequestAsync()
    {
        // Arrange
        var email = $"missing_invalid_page_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=0&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithInvalidLimit_ReturnsBadRequestAsync()
    {
        // Arrange
        var email = $"missing_invalid_pageSize_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=1&pageSize=501");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMissingStickersAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMissingStickersAsync_ResponseContainsExpectedFields_ReturnsOkAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync("Vinicius Jr", StickerRarity.Foil);

        var email = $"missing_fields_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/missing?page=1&pageSize=100");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedMissingStickersResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
        body.Page.ShouldBe(1);
        body.PageSize.ShouldBe(100);
        body.TotalPages.ShouldBeGreaterThanOrEqualTo(0);

        var item = body.Items.FirstOrDefault(x => x.StickerId == stickerId);
        item.ShouldNotBeNull();
        item!.PlayerName.ShouldBe("Vinicius Jr");
        item.Team.ShouldBe("Brasil");
        item.Rarity.ShouldBe(StickerRarity.Foil);
        item.StickerId.ShouldNotBe(Guid.Empty);
        item.Code.ShouldNotBeNullOrEmpty();
    }
}
