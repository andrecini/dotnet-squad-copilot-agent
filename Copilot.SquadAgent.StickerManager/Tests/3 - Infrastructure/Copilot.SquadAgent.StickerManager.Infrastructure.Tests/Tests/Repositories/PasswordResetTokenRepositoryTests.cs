using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.PasswordResetToken;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class PasswordResetTokenRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<Domain.Entities.User> SeedUserAsync(AppDbContext dbContext)
    {
        var user = UserEntityMock.Valid();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task CreateAsync_ValidToken_ReturnsSuccessAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Valid(user.Id);
        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.CreateAsync(token, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Token.ShouldBe(token.Token);
    }

    [Fact]
    public async Task CreateAsync_ValidToken_PersistsInDatabaseAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Valid(user.Id);
        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        await repository.CreateAsync(token, CancellationToken.None);

        // Assert
        var persisted = await dbContext.PasswordResetTokens.FindAsync(token.Id);
        persisted.ShouldNotBeNull();
        persisted!.Token.ShouldBe(token.Token);
        persisted.IsUsed.ShouldBeFalse();
    }

    [Fact]
    public async Task GetValidByTokenAsync_ValidToken_ReturnsSuccessAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Valid(user.Id);
        await dbContext.PasswordResetTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.GetValidByTokenAsync(token.Token, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Token.ShouldBe(token.Token);
    }

    [Fact]
    public async Task GetValidByTokenAsync_ExpiredToken_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Expired(user.Id);
        await dbContext.PasswordResetTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.GetValidByTokenAsync(token.Token, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetValidByTokenAsync_AlreadyUsedToken_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.AlreadyUsed(user.Id);
        await dbContext.PasswordResetTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.GetValidByTokenAsync(token.Token, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
    }

    [Fact]
    public async Task GetValidByTokenAsync_NonExistentToken_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.GetValidByTokenAsync("token-inexistente", CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
    }

    [Fact]
    public async Task MarkAsUsedAsync_ValidToken_SetsIsUsedToTrueAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Valid(user.Id);
        await dbContext.PasswordResetTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.MarkAsUsedAsync(token, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.IsUsed.ShouldBeTrue();
    }

    [Fact]
    public async Task MarkAsUsedAsync_ValidToken_PersistsIsUsedInDatabaseAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Valid(user.Id);
        await dbContext.PasswordResetTokens.AddAsync(token);
        await dbContext.SaveChangesAsync();

        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        await repository.MarkAsUsedAsync(token, CancellationToken.None);

        // Assert
        var persisted = await dbContext.PasswordResetTokens.FindAsync(token.Id);
        persisted.ShouldNotBeNull();
        persisted!.IsUsed.ShouldBeTrue();
    }

    [Fact]
    public async Task MarkAsUsedAsync_TokenNotInDatabase_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var token = PasswordResetTokenEntityMock.Valid(user.Id);
        // Não persiste — usa apenas o objeto em memória

        var repository = new PasswordResetTokenRepository(dbContext);

        // Act
        var result = await repository.MarkAsUsedAsync(token, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
    }
}
