using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;
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
public class AddToCollectionEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    private async Task<Guid> SeedStickerAsync()
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
            PlayerName = "Neymar",
            Rarity = StickerRarity.Base,
            TeamId = team.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.Stickers.Add(sticker);
        await db.SaveChangesAsync();

        return sticker.Id;
    }

    [Fact]
    public async Task AddStickerAsync_NewSticker_ReturnsCreatedWithCollectionIdAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"col_new_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = AddToCollectionRequestMock.Valid(stickerId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AddToCollectionResponse>();
        body.ShouldNotBeNull();
        body.CollectionId.ShouldNotBe(Guid.Empty);
        body.StickerId.ShouldBe(stickerId);
        body.QuantityOwned.ShouldBe(1);
        body.QuantityDuplicate.ShouldBe(0);
    }

    [Fact]
    public async Task AddStickerAsync_ExistingSticker_IncrementsQuantityAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"col_dup_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = AddToCollectionRequestMock.Valid(stickerId);

        // Adiciona a primeira vez
        await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Act — adiciona a segunda vez
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AddToCollectionResponse>();
        body.ShouldNotBeNull();
        body.QuantityOwned.ShouldBe(2);
        body.QuantityDuplicate.ShouldBe(1);
    }

    [Fact]
    public async Task AddStickerAsync_StickerNotFound_ReturnsProblemAsync()
    {
        // Arrange
        var email = $"col_notfound_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = AddToCollectionRequestMock.Valid(Guid.NewGuid());

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddStickerAsync_MaxDuplicatesReached_ReturnsUnprocessableEntityAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"col_max_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = AddToCollectionRequestMock.Valid(stickerId);

        // Adiciona 11 vezes (1 original + 10 duplicatas = limite atingido)
        for (var i = 0; i <= 10; i++)
            await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Act — 12ª adição deve ser rejeitada
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Assert
        ((int)response.StatusCode).ShouldBe(422);
    }

    [Fact]
    public async Task AddStickerAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var request = AddToCollectionRequestMock.Valid(Guid.NewGuid());

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddStickerAsync_EmptyStickerId_ReturnsBadRequestAsync()
    {
        // Arrange
        var email = $"col_empty_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = AddToCollectionRequestMock.WithEmptyStickerId();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
