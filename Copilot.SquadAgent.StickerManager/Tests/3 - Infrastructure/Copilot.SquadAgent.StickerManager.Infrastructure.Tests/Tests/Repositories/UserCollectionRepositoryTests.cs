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
}
