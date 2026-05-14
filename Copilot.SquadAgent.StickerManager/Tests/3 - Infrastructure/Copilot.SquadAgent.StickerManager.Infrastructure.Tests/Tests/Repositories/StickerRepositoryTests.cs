using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class StickerRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetByIdAsync_StickerExists_ReturnsSuccessAsync()
    {
        // Arrange
        var sticker = StickerEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(sticker.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Id.ShouldBe(sticker.Id);
        result.Value.Code.ShouldBe(sticker.Code);
    }

    [Fact]
    public async Task GetByIdAsync_StickerDoesNotExist_ReturnsNotFoundAsync()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new StickerRepository(dbContext);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
        result.Message.ShouldBe("Figurinha não encontrada.");
    }

    [Fact]
    public async Task GetByIdAsync_StickerExists_ReturnsCorrectPlayerNameAsync()
    {
        // Arrange
        var sticker = StickerEntityMock.Valid();
        await using var dbContext = CreateDbContext();
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(sticker.Id, CancellationToken.None);

        // Assert
        result.Value!.PlayerName.ShouldBe(sticker.PlayerName);
    }
}
