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
public class RemoveStickerFromCollectionEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
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
    public async Task RemoveStickerAsync_ValidIdAndOwner_ReturnsNoContentAsync()
    {
        // Arrange
        var stickerId = await SeedStickerAsync();

        var email = $"remove_success_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var collectionId = await AddStickerToCollectionAsync(stickerId);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/collection/{collectionId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveStickerAsync_NonExistentId_ReturnsNotFoundAsync()
    {
        // Arrange
        var email = $"remove_notfound_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/collection/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveStickerAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var anyId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/collection/{anyId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveStickerAsync_RecordBelongsToAnotherUser_ReturnsForbiddenAsync()
    {
        // Arrange — usuário A adiciona a figurinha à própria coleção
        var stickerId = await SeedStickerAsync();

        var ownerEmail = $"remove_owner_{Guid.NewGuid():N}@test.com";
        var ownerToken = await AuthHelper.GetTokenAsync(_client, ownerEmail);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);

        var collectionId = await AddStickerToCollectionAsync(stickerId);

        // Usuário B tenta remover a entrada do usuário A
        var otherEmail = $"remove_other_{Guid.NewGuid():N}@test.com";
        var otherToken = await AuthHelper.GetTokenAsync(_client, otherEmail);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherToken);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/collection/{collectionId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
