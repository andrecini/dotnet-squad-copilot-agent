using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.UserCollection;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class UserCollectionRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetByUserAndStickerAsync_EntryExists_ReturnsSuccessWithEntityAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stickerId = Guid.NewGuid();
        var userCollection = UserCollectionEntityMock.Valid(userId, stickerId);

        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetByUserAndStickerAsync(userId, stickerId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.UserId.ShouldBe(userId);
        result.Value.StickerId.ShouldBe(stickerId);
    }

    [Fact]
    public async Task GetByUserAndStickerAsync_EntryDoesNotExist_ReturnsSuccessWithNullAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetByUserAndStickerAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidEntry_ReturnsSuccessAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.CreateAsync(userCollection, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.UserId.ShouldBe(userCollection.UserId);
        result.Value.StickerId.ShouldBe(userCollection.StickerId);
    }

    [Fact]
    public async Task CreateAsync_ValidEntry_PersistsEntryInDatabaseAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        var repository = new UserCollectionRepository(dbContext);

        // Act
        await repository.CreateAsync(userCollection, CancellationToken.None);

        // Assert
        var persisted = await dbContext.UserCollections.FindAsync(userCollection.Id);
        persisted.ShouldNotBeNull();
        persisted!.UserId.ShouldBe(userCollection.UserId);
    }

    [Fact]
    public async Task UpdateAsync_ValidEntry_ReturnsSuccessAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        userCollection.QuantityOwned = 2;
        userCollection.QuantityDuplicate = 1;

        // Act
        var result = await repository.UpdateAsync(userCollection, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.QuantityOwned.ShouldBe(2);
        result.Value.QuantityDuplicate.ShouldBe(1);
    }

    [Fact]
    public async Task UpdateAsync_ValidEntry_PersistsChangesInDatabaseAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        userCollection.QuantityOwned = 5;
        userCollection.QuantityDuplicate = 4;

        // Act
        await repository.UpdateAsync(userCollection, CancellationToken.None);

        // Assert
        var persisted = await dbContext.UserCollections.FindAsync(userCollection.Id);
        persisted.ShouldNotBeNull();
        persisted!.QuantityOwned.ShouldBe(5);
        persisted.QuantityDuplicate.ShouldBe(4);
    }

    [Fact]
    public async Task GetByUserAndStickerAsync_DifferentUserSameSticker_ReturnsNullAsync()
    {
        // Arrange
        var stickerId = Guid.NewGuid();
        var userCollection = UserCollectionEntityMock.Valid(Guid.NewGuid(), stickerId);

        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetByUserAndStickerAsync(Guid.NewGuid(), stickerId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_EntryExists_ReturnsSuccessWithEntityAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(userCollection.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Id.ShouldBe(userCollection.Id);
    }

    [Fact]
    public async Task GetByIdAsync_EntryDoesNotExist_ReturnsSuccessWithNullAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_DeletedEntry_ReturnsSuccessWithNullAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stickerId = Guid.NewGuid();
        var userCollection = UserCollectionEntityMock.Deleted(userId, stickerId);

        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(userCollection.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task SoftDeleteAsync_ValidEntry_SetsDeletedAtAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        await repository.SoftDeleteAsync(userCollection, CancellationToken.None);

        // Assert
        var persisted = await dbContext.UserCollections.FindAsync(userCollection.Id);
        persisted.ShouldNotBeNull();
        persisted!.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task SoftDeleteAsync_ValidEntry_ReturnsSuccessAsync()
    {
        // Arrange
        var userCollection = UserCollectionEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);

        // Act
        var result = await repository.SoftDeleteAsync(userCollection, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ListByUserAsync_NoFilters_ReturnsAllUserItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var userCollection = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker.Id, QuantityOwned = 1, QuantityDuplicate = 0 };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].StickerId.ShouldBe(sticker.Id);
    }

    [Fact]
    public async Task ListByUserAsync_WithTeamFilter_ReturnsFilteredItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team1 = TeamEntityMock.Valid();
        var team2 = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };
        var sticker1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team1.Id };
        var sticker2 = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi", Rarity = StickerRarity.Base, TeamId = team2.Id };
        var collection1 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker1.Id, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collection2 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker2.Id, QuantityOwned = 1, QuantityDuplicate = 0 };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(team1, team2);
        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2);
        await dbContext.UserCollections.AddRangeAsync(collection1, collection2);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, TeamId = team1.Id, Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].StickerId.ShouldBe(sticker1.Id);
    }

    [Fact]
    public async Task ListByUserAsync_WithRarityFilter_ReturnsFilteredItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerBase = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var stickerFoil = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Foil, TeamId = team.Id };
        var collectionBase = new Domain.Entities.UserCollection { UserId = userId, StickerId = stickerBase.Id, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collectionFoil = new Domain.Entities.UserCollection { UserId = userId, StickerId = stickerFoil.Id, QuantityOwned = 1, QuantityDuplicate = 0 };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerBase, stickerFoil);
        await dbContext.UserCollections.AddRangeAsync(collectionBase, collectionFoil);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, Rarity = StickerRarity.Foil, Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].Rarity.ShouldBe(StickerRarity.Foil);
    }

    [Fact]
    public async Task ListByUserAsync_SortByPlayerName_ReturnsSortedItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerA = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Alisson", Rarity = StickerRarity.Base, TeamId = team.Id };
        var stickerV = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var collectionA = new Domain.Entities.UserCollection { UserId = userId, StickerId = stickerA.Id, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collectionV = new Domain.Entities.UserCollection { UserId = userId, StickerId = stickerV.Id, QuantityOwned = 1, QuantityDuplicate = 0 };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerA, stickerV);
        await dbContext.UserCollections.AddRangeAsync(collectionA, collectionV);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, Sort = "player_name", Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
        result.Value[0].PlayerName.ShouldBe("Alisson");
        result.Value[1].PlayerName.ShouldBe("Vinicius Jr");
    }

    [Fact]
    public async Task ListByUserAsync_SortByAcquiredAt_ReturnsSortedItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var sticker2 = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var collection1 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker1.Id, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collection2 = new Domain.Entities.UserCollection { UserId = userId, StickerId = sticker2.Id, QuantityOwned = 1, QuantityDuplicate = 0 };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2);
        await dbContext.UserCollections.AddRangeAsync(collection1, collection2);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, Sort = "acquired_at", Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListByUserAsync_Pagination_ReturnsCorrectPageAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickers = Enumerable.Range(1, 5).Select(i => new Domain.Entities.Sticker
        {
            Code = $"BRA00{i}",
            PlayerName = $"Player {i:D2}",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        }).ToList();
        var collections = stickers.Select(s => new Domain.Entities.UserCollection
        {
            UserId = userId,
            StickerId = s.Id,
            QuantityOwned = 1,
            QuantityDuplicate = 0
        }).ToList();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickers);
        await dbContext.UserCollections.AddRangeAsync(collections);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, Page = 2, Limit = 2 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListByUserAsync_DeletedEntries_AreNotReturnedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerActive = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var stickerDeleted = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = team.Id };
        var collectionActive = new Domain.Entities.UserCollection { UserId = userId, StickerId = stickerActive.Id, QuantityOwned = 1, QuantityDuplicate = 0 };
        var collectionDeleted = new Domain.Entities.UserCollection
        {
            UserId = userId,
            StickerId = stickerDeleted.Id,
            QuantityOwned = 1,
            QuantityDuplicate = 0,
            DeletedAt = DateTime.UtcNow.AddDays(-1)
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerActive, stickerDeleted);
        await dbContext.UserCollections.AddRangeAsync(collectionActive, collectionDeleted);
        await dbContext.SaveChangesAsync();

        var repository = new UserCollectionRepository(dbContext);
        var filter = new ListCollectionModel { UserId = userId, Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].StickerId.ShouldBe(stickerActive.Id);
    }
}
