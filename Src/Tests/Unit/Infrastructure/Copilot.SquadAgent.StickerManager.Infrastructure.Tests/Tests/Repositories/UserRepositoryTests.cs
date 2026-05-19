using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.User;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class UserRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ExistsByEmailAsync_UserExists_ReturnsTrueAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.ExistsByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_UserDoesNotExist_ReturnsFalseAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.ExistsByEmailAsync("naoexiste@email.com", CancellationToken.None);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task CreateAsync_ValidUser_ReturnsSuccessAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.CreateAsync(user, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Email.ShouldBe(user.Email);
        result.Value.Name.ShouldBe(user.Name);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_PersistsUserInDatabaseAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        var repository = new UserRepository(dbContext);

        // Act
        await repository.CreateAsync(user, CancellationToken.None);

        // Assert
        var persisted = await dbContext.Users.FindAsync(user.Id);
        persisted.ShouldNotBeNull();
        persisted!.Email.ShouldBe(user.Email);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_SetsCreatedAtTimestampAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        var repository = new UserRepository(dbContext);
        var before = DateTime.UtcNow;

        // Act
        var result = await repository.CreateAsync(user, CancellationToken.None);

        // Assert
        result.Value!.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }
}
