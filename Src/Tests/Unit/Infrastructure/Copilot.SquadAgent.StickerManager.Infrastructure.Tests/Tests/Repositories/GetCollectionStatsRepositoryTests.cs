using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.UserCollection;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;
using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class GetCollectionStatsRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetStatsAsync_WithPopulatedCollection_ReturnsCorrectTotalsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker1 = new StickerEntity { Code = "BRA001", PlayerName = "Player 1", Rarity = StickerRarity.Base, TeamId = team.Id, Team = team };
        var sticker2 = new StickerEntity { Code = "BRA002", PlayerName = "Player 2", Rarity = StickerRarity.Base, TeamId = team.Id, Team = team };
        var sticker3 = new StickerEntity { Code = "BRA003", PlayerName = "Player 3", Rarity = StickerRarity.Foil, TeamId = team.Id, Team = team };

        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2, sticker3);
        await dbContext.SaveChangesAsync();

        var col1 = new UserCollectionEntity { UserId = userId, StickerId = sticker1.Id, Sticker = sticker1, QuantityOwned = 2, QuantityDuplicate = 1 };
        var col2 = new UserCollectionEntity { UserId = userId, StickerId = sticker2.Id, Sticker = sticker2, QuantityOwned = 1, QuantityDuplicate = 0 };

        await dbContext.UserCollections.AddRangeAsync(col1, col2);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalStickers.ShouldBe(3);
        result.Value.TotalOwned.ShouldBe(2);
        result.Value.TotalMissing.ShouldBe(1);
        result.Value.Duplicates.ShouldBe(1);
    }

    [Fact]
    public async Task GetStatsAsync_WithEmptyCollection_ReturnsZeroOwnedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker = StickerEntityMock.ValidWithTeam(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalOwned.ShouldBe(0);
        result.Value.TotalMissing.ShouldBe(1);
        result.Value.CompletionPercentage.ShouldBe(0);
        result.Value.Duplicates.ShouldBe(0);
        result.Value.ByTeam.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetStatsAsync_DeletedEntriesIgnored_NotCountedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker = StickerEntityMock.ValidWithTeam(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var deleted = new UserCollectionEntity
        {
            UserId            = userId,
            StickerId         = sticker.Id,
            Sticker           = sticker,
            QuantityOwned     = 1,
            QuantityDuplicate = 0,
            DeletedAt         = DateTime.UtcNow.AddDays(-1)
        };

        await dbContext.UserCollections.AddAsync(deleted);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalOwned.ShouldBe(0);
    }

    [Fact]
    public async Task GetStatsAsync_ByTeamContainsCorrectGrouping_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBra = TeamEntityMock.Valid();
        var teamArg = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBra, teamArg);

        var stickerBra = new StickerEntity { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = teamBra.Id, Team = teamBra };
        var stickerArg = new StickerEntity { Code = "ARG001", PlayerName = "Messi", Rarity = StickerRarity.Base, TeamId = teamArg.Id, Team = teamArg };

        await dbContext.Stickers.AddRangeAsync(stickerBra, stickerArg);
        await dbContext.SaveChangesAsync();

        var colBra = new UserCollectionEntity { UserId = userId, StickerId = stickerBra.Id, Sticker = stickerBra, QuantityOwned = 1, QuantityDuplicate = 0 };

        await dbContext.UserCollections.AddAsync(colBra);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.ByTeam.Count.ShouldBe(1);
        result.Value.ByTeam.First().TeamName.ShouldBe("Brasil");
        result.Value.ByTeam.First().Owned.ShouldBe(1);
    }

    [Fact]
    public async Task GetStatsAsync_NoStickersInDatabase_ReturnsZeroCompletionPercentageAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();

        await using var dbContext = CreateDbContext();
        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalStickers.ShouldBe(0);
        result.Value.TotalOwned.ShouldBe(0);
        result.Value.TotalMissing.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0);
        result.Value.ByTeam.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetStatsAsync_TotalMissingNeverNegative_WhenOwnedExceedsTotalAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker = new StickerEntity { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id, Team = team };
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        // Adiciona dois registros de collection para o mesmo usuário (sem sticker duplicado real),
        // simulando inconsistência que faria totalMissing = -1 sem o guard clause.
        // Como o InMemory não impõe unicidade de negócio, adicionamos via SQL-like trick:
        // uma collection de outro sticker (inexistente no db) para forçar totalOwned > totalStickers
        var col1 = new UserCollectionEntity { UserId = userId, StickerId = sticker.Id, Sticker = sticker, QuantityOwned = 1, QuantityDuplicate = 0 };
        var col2 = new UserCollectionEntity { UserId = userId, StickerId = Guid.NewGuid(), QuantityOwned = 1, QuantityDuplicate = 0 };

        await dbContext.UserCollections.AddRangeAsync(col1, col2);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        // totalMissing = 1 - 2 = -1, mas o guard clause retorna 0
        result.Value!.TotalMissing.ShouldBe(0);
    }

    [Fact]
    public async Task GetStatsAsync_ByTeamCompletionPercentage_IsCalculatedCorrectlyAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);

        var sticker1 = new StickerEntity { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id, Team = team };
        var sticker2 = new StickerEntity { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id, Team = team };
        var sticker3 = new StickerEntity { Code = "BRA003", PlayerName = "Marquinhos", Rarity = StickerRarity.Foil, TeamId = team.Id, Team = team };

        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2, sticker3);
        await dbContext.SaveChangesAsync();

        var col1 = new UserCollectionEntity { UserId = userId, StickerId = sticker1.Id, Sticker = sticker1, QuantityOwned = 1, QuantityDuplicate = 0 };

        await dbContext.UserCollections.AddAsync(col1);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.ByTeam.Count.ShouldBe(1);
        var teamStats = result.Value.ByTeam.First();
        teamStats.Total.ShouldBe(3);
        teamStats.Owned.ShouldBe(1);
        teamStats.CompletionPercentage.ShouldBe(33.3);
    }
}
