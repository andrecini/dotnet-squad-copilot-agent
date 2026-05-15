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
public class ToggleDuplicateEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
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

    private async Task<Guid> AddStickerToCollectionAsync(Guid stickerId)
    {
        var request = AddToCollectionRequestMock.Valid(stickerId);
        var response = await _client.PostAsJsonAsync("/api/v1/collection", request);
        var body = await response.Content.ReadFromJsonAsync<AddToCollectionResponse>();
        return body!.CollectionId;
    }

    [Fact]
    public async Task ToggleDuplicateAsync_MarkAction_DecreasesOwnedAndIncreasesDuplicateAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"toggle_mark_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Adiciona duas vezes: QuantityOwned=2, QuantityDuplicate=1
        await _client.PostAsJsonAsync("/api/v1/collection", AddToCollectionRequestMock.Valid(stickerId));
        var collectionId = await AddStickerToCollectionAsync(stickerId);

        var request = ToggleDuplicateRequestMock.Mark();

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ToggleDuplicateResponse>();
        body.ShouldNotBeNull();
        body!.QuantityOwned.ShouldBe(1);
        body.QuantityDuplicate.ShouldBe(2);
    }

    [Fact]
    public async Task ToggleDuplicateAsync_UnmarkAction_DecreasesDuplicateAndIncreasesOwnedAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"toggle_unmark_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Adiciona duas vezes: QuantityOwned=2, QuantityDuplicate=1
        await _client.PostAsJsonAsync("/api/v1/collection", AddToCollectionRequestMock.Valid(stickerId));
        var collectionId = await AddStickerToCollectionAsync(stickerId);

        // Primeiro marca como duplicata: QuantityOwned=1, QuantityDuplicate=2
        await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", ToggleDuplicateRequestMock.Mark());

        var request = ToggleDuplicateRequestMock.Unmark();

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ToggleDuplicateResponse>();
        body.ShouldNotBeNull();
        body!.QuantityOwned.ShouldBe(2);
        body.QuantityDuplicate.ShouldBe(1);
    }

    [Fact]
    public async Task ToggleDuplicateAsync_MarkWithQuantityOwnedZero_ReturnsUnprocessableEntityAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"toggle_mark_zero_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Adiciona uma vez: QuantityOwned=1, QuantityDuplicate=0
        var collectionId = await AddStickerToCollectionAsync(stickerId);

        // Marca a única figurinha como duplicata: QuantityOwned=0, QuantityDuplicate=1
        await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", ToggleDuplicateRequestMock.Mark());

        // Act — tenta marcar novamente com QuantityOwned=0
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", ToggleDuplicateRequestMock.Mark());

        // Assert
        ((int)response.StatusCode).ShouldBe(422);
    }

    [Fact]
    public async Task ToggleDuplicateAsync_UnmarkWithQuantityDuplicateZero_ReturnsUnprocessableEntityAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"toggle_unmark_zero_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Adiciona uma vez: QuantityOwned=1, QuantityDuplicate=0
        var collectionId = await AddStickerToCollectionAsync(stickerId);

        // Act — tenta desmarcar com QuantityDuplicate=0
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", ToggleDuplicateRequestMock.Unmark());

        // Assert
        ((int)response.StatusCode).ShouldBe(422);
    }

    [Fact]
    public async Task ToggleDuplicateAsync_NonExistentCollectionId_ReturnsNotFoundAsync()
    {
        // Arrange
        var email = $"toggle_notfound_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var nonExistentId = Guid.NewGuid();
        var request = ToggleDuplicateRequestMock.Mark();

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{nonExistentId}/duplicate", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ToggleDuplicateAsync_RecordBelongsToAnotherUser_ReturnsForbiddenAsync()
    {
        // Arrange — usuário A adiciona a figurinha
        var stickerId = await SeedStickerAsync();

        var ownerEmail = $"toggle_owner_{Guid.NewGuid():N}@test.com";
        var ownerToken = await AuthHelper.GetTokenAsync(_client, ownerEmail);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);

        var collectionId = await AddStickerToCollectionAsync(stickerId);

        // Usuário B tenta fazer toggle no registro do usuário A
        var otherEmail = $"toggle_other_{Guid.NewGuid():N}@test.com";
        var otherToken = await AuthHelper.GetTokenAsync(_client, otherEmail);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherToken);

        var request = ToggleDuplicateRequestMock.Mark();

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{collectionId}/duplicate", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ToggleDuplicateAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var request = ToggleDuplicateRequestMock.Mark();

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/collection/{Guid.NewGuid()}/duplicate", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
