using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.User;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class UserProfileRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetByIdAsync_UserExists_ReturnsSuccessAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Id.ShouldBe(user.Id);
        result.Value.Email.ShouldBe(user.Email);
    }

    [Fact]
    public async Task GetByIdAsync_UserDoesNotExist_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new UserRepository(dbContext);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
        result.Message.ShouldBe("Usuário não encontrado.");
    }

    [Fact]
    public async Task UpdateAsync_ValidUser_ReturnsSuccessAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);
        user.Name = "Nome Atualizado";

        // Act
        var result = await repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Name.ShouldBe("Nome Atualizado");
    }

    [Fact]
    public async Task UpdateAsync_ValidUser_PersistsChangesInDatabaseAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);
        user.Name = "Nome Persistido";
        user.Email = "atualizado@email.com";

        // Act
        await repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        var persisted = await dbContext.Users.FindAsync(user.Id);
        persisted.ShouldNotBeNull();
        persisted!.Name.ShouldBe("Nome Persistido");
        persisted.Email.ShouldBe("atualizado@email.com");
    }

    [Fact]
    public async Task UpdateAsync_ValidUser_SetsUpdatedAtTimestampAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);
        user.Name = "Nome Atualizado";
        var before = DateTime.UtcNow;

        // Act
        var result = await repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        result.Value!.UpdatedAt.ShouldNotBeNull();
        result.Value.UpdatedAt!.Value.ShouldBeGreaterThanOrEqualTo(before);
    }
}
