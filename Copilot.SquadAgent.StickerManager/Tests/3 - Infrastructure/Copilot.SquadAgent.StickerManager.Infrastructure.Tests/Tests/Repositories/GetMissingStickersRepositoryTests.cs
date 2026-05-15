using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class GetMissingStickersRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ListMissingByUserAsync_UserOwnsNoStickers_ReturnsAllStickersAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].StickerId.ShouldBe(sticker.Id);
    }

    [Fact]
    public async Task ListMissingByUserAsync_UserOwnsAllStickers_ReturnsEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var userCollection = new Domain.Entities.UserCollection
        {
            UserId = userId,
            StickerId = sticker.Id,
            QuantityOwned = 1,
            QuantityDuplicate = 0
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListMissingByUserAsync_UserOwnsSomeStickers_ReturnsOnlyMissingOnesAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerOwned = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var stickerMissing = new Domain.Entities.Sticker
        {
            Code = "BRA002",
            PlayerName = "Vinicius Jr",
            Rarity = StickerRarity.Foil,
            TeamId = team.Id
        };
        var userCollection = new Domain.Entities.UserCollection
        {
            UserId = userId,
            StickerId = stickerOwned.Id,
            QuantityOwned = 1,
            QuantityDuplicate = 0
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerOwned, stickerMissing);
        await dbContext.UserCollections.AddAsync(userCollection);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].StickerId.ShouldBe(stickerMissing.Id);
    }

    [Fact]
    public async Task ListMissingByUserAsync_SoftDeletedCollectionEntry_TreatsAsMissingAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var deletedUserCollection = new Domain.Entities.UserCollection
        {
            UserId = userId,
            StickerId = sticker.Id,
            QuantityOwned = 1,
            QuantityDuplicate = 0,
            DeletedAt = DateTime.UtcNow.AddDays(-1)
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.UserCollections.AddAsync(deletedUserCollection);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value[0].StickerId.ShouldBe(sticker.Id);
    }

    [Fact]
    public async Task ListMissingByUserAsync_DefaultSort_OrdersByRarityDescAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerBase = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var stickerFoil = new Domain.Entities.Sticker
        {
            Code = "BRA002",
            PlayerName = "Vinicius Jr",
            Rarity = StickerRarity.Foil,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerBase, stickerFoil);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListMissingByUserAsync_SortByTeam_OrdersByTeamNameAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil = new Domain.Entities.Team { Name = "Brasil", Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };
        var stickerBrazil = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = teamBrazil.Id
        };
        var stickerArgentina = new Domain.Entities.Sticker
        {
            Code = "ARG001",
            PlayerName = "Messi",
            Rarity = StickerRarity.Base,
            TeamId = teamArgentina.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBrazil, stickerArgentina);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId, Sort = "team" };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
        result.Value[0].TeamName.ShouldBe("Argentina");
        result.Value[1].TeamName.ShouldBe("Brasil");
    }

    [Fact]
    public async Task ListMissingByUserAsync_SortByNumber_OrdersByCodeAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerC = new Domain.Entities.Sticker
        {
            Code = "BRA003",
            PlayerName = "Casemiro",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var stickerA = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Alisson",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerC, stickerA);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId, Sort = "number" };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
        result.Value[0].Code.ShouldBe("BRA001");
        result.Value[1].Code.ShouldBe("BRA003");
    }

    [Fact]
    public async Task ListMissingByUserAsync_ResponseContainsCorrectFields_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = new Domain.Entities.Team { Name = "Brasil", Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var sticker = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Marquinhos",
            Rarity = StickerRarity.Foil,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId, Page = 1, Limit = 100 };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        var item = result.Value[0];
        item.StickerId.ShouldBe(sticker.Id);
        item.Code.ShouldBe("BRA001");
        item.PlayerName.ShouldBe("Marquinhos");
        item.TeamName.ShouldBe("Brasil");
        item.TeamCode.ShouldBe("BRA");
        item.Rarity.ShouldBe(StickerRarity.Foil);
    }

    [Fact]
    public async Task ListMissingByUserAsync_WithPaginationLimit1_ReturnsOnlyOneItemAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker1 = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var sticker2 = new Domain.Entities.Sticker
        {
            Code = "BRA002",
            PlayerName = "Vinicius Jr",
            Rarity = StickerRarity.Foil,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(sticker1, sticker2);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId, Page = 1, Limit = 1 };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ListMissingByUserAsync_WithPage2Limit1_ReturnsSecondItemAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickerA = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Alisson",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };
        var stickerB = new Domain.Entities.Sticker
        {
            Code = "BRA002",
            PlayerName = "Casemiro",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickerA, stickerB);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filterPage1 = new MissingStickersModel { UserId = userId, Sort = "number", Page = 1, Limit = 1 };
        var filterPage2 = new MissingStickersModel { UserId = userId, Sort = "number", Page = 2, Limit = 1 };

        // Act
        var resultPage1 = await repository.ListMissingByUserAsync(filterPage1, CancellationToken.None);
        var resultPage2 = await repository.ListMissingByUserAsync(filterPage2, CancellationToken.None);

        // Assert
        resultPage1.IsSuccess.ShouldBeTrue();
        resultPage1.Value!.Count.ShouldBe(1);
        resultPage1.Value[0].Code.ShouldBe("BRA001");

        resultPage2.IsSuccess.ShouldBeTrue();
        resultPage2.Value!.Count.ShouldBe(1);
        resultPage2.Value[0].Code.ShouldBe("BRA002");
    }

    [Fact]
    public async Task ListMissingByUserAsync_PageBeyondLastPage_ReturnsEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = new Domain.Entities.Sticker
        {
            Code = "BRA001",
            PlayerName = "Neymar Jr",
            Rarity = StickerRarity.Base,
            TeamId = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var filter = new MissingStickersModel { UserId = userId, Page = 99, Limit = 100 };

        // Act
        var result = await repository.ListMissingByUserAsync(filter, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(0);
    }
}
