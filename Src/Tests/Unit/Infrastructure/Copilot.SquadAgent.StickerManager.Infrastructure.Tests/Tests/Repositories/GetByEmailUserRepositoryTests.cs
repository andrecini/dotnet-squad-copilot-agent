using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.User;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class GetByEmailUserRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetByEmailAsync_UserExists_ReturnsSuccessAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.GetByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Email.ShouldBe(user.Email);
        result.Value.Name.ShouldBe(user.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_UserDoesNotExist_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.GetByEmailAsync("naoexiste@email.com", CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetByEmailAsync_DeletedUser_ReturnsNotFoundAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        user.DeletedAt = DateTime.UtcNow;

        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.GetByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetByEmailAsync_UserExists_ReturnsPasswordHashAsync()
    {
        // Arrange
        var user = UserEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);

        // Act
        var result = await repository.GetByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.PasswordHash.ShouldBe(user.PasswordHash);
    }
}
