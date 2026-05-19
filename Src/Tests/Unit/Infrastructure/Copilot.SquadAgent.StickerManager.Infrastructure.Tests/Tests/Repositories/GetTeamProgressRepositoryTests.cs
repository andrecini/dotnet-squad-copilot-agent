using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class GetTeamProgressRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetTeamProgressAsync_TeamDoesNotExist_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new StickerRepository(dbContext);
        var nonExistentTeamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = await repository.GetTeamProgressAsync(nonExistentTeamId, userId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
        result.Message.ShouldBe("Time não encontrado.");
    }

    [Fact]
    public async Task GetTeamProgressAsync_TeamExistsWithOwnedStickers_ReturnsCorrectProgressAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var sticker2 = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var sticker3 = new Domain.Entities.Sticker { Code = "BRA003", PlayerName = "Marquinhos", Rarity = StickerRarity.Foil, TeamId = team.Id };

        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2, sticker3);
        await dbContext.SaveChangesAsync();

        var collection1 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker1.Id, Sticker = sticker1, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collection2 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker2.Id, Sticker = sticker2, QuantityOwned = 1, QuantityDuplicate = 0 };

        await dbContext.UserCollections.AddRangeAsync(collection1, collection2);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetTeamProgressAsync(team.Id, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TeamId.ShouldBe(team.Id);
        result.Value.TeamName.ShouldBe("Brasil");
        result.Value.TotalStickers.ShouldBe(3);
        result.Value.OwnedCount.ShouldBe(2);
        result.Value.CompletionPercentage.ShouldBe(66.7);
    }

    [Fact]
    public async Task GetTeamProgressAsync_TeamExistsWithNoOwnedStickers_ReturnsZeroProgressAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var sticker2 = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id };

        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetTeamProgressAsync(team.Id, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalStickers.ShouldBe(2);
        result.Value.OwnedCount.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_TeamExistsWithNoStickers_ReturnsZeroCompletionAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetTeamProgressAsync(team.Id, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalStickers.ShouldBe(0);
        result.Value.OwnedCount.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_SoftDeletedCollectionEntries_AreNotCountedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var deletedCollection = new Domain.Entities.UserCollection
        {
            UserId            = userId,
            StickerId         = sticker.Id,
            Sticker           = sticker,
            QuantityOwned     = 1,
            QuantityDuplicate = 0,
            DeletedAt         = DateTime.UtcNow.AddDays(-1)
        };

        await dbContext.UserCollections.AddAsync(deletedCollection);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetTeamProgressAsync(team.Id, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalStickers.ShouldBe(1);
        result.Value.OwnedCount.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_AllStickersOwned_Returns100PercentAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var sticker2 = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id };

        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2);
        await dbContext.SaveChangesAsync();

        var collection1 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker1.Id, Sticker = sticker1, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collection2 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker2.Id, Sticker = sticker2, QuantityOwned = 1, QuantityDuplicate = 0 };

        await dbContext.UserCollections.AddRangeAsync(collection1, collection2);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetTeamProgressAsync(team.Id, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalStickers.ShouldBe(2);
        result.Value.OwnedCount.ShouldBe(2);
        result.Value.CompletionPercentage.ShouldBe(100.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_StickersFromOtherTeam_AreNotCountedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBra = TeamEntityMock.Valid();
        var teamArg = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBra, teamArg);

        var stickerBra = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = teamBra.Id };
        var stickerArg = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi", Rarity = StickerRarity.Base, TeamId = teamArg.Id };

        await dbContext.Stickers.AddRangeAsync(stickerBra, stickerArg);
        await dbContext.SaveChangesAsync();

        var collectionArg = new Domain.Entities.UserCollection { UserId = userId, StickerId = stickerArg.Id, Sticker = stickerArg, QuantityOwned = 1, QuantityDuplicate = 0 };
        await dbContext.UserCollections.AddAsync(collectionArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetTeamProgressAsync(teamBra.Id, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalStickers.ShouldBe(1);
        result.Value.OwnedCount.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0.0);
    }
}
