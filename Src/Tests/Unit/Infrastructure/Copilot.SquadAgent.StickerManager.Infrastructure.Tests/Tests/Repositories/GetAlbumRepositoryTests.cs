using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;
using Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.Tests.Repositories;

public class GetAlbumRepositoryTests
{
    private static AppDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static AlbumQueryModel DefaultQuery(Guid userId, int page = 1, int pageSize = 20, bool sortByTeam = false, Guid? teamId = null) => new()
    {
        UserId     = userId,
        Page       = page,
        PageSize   = pageSize,
        SortByTeam = sortByTeam,
        TeamId     = teamId
    };

    [Fact]
    public async Task GetAlbumAsync_EmptyDatabase_ReturnsEmptyPagedResultAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
        result.Value.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_UserOwnsNoStickers_AllReturnedWithOwnedFalseAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = StickerEntityMock.ValidWithTeam(team);

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(1);
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items[0].Owned.ShouldBeFalse();
        result.Value.Items[0].QuantityOwned.ShouldBe(0);
        result.Value.Items[0].QuantityDuplicate.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_UserOwnsSticker_OwnedFlagIsTrueAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = StickerEntityMock.ValidWithTeam(team);
        var collection = new Domain.Entities.UserCollection
        {
            UserId            = userId,
            StickerId         = sticker.Id,
            QuantityOwned     = 2,
            QuantityDuplicate = 1
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.UserCollections.AddAsync(collection);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var item = result.Value!.Items[0];
        item.Owned.ShouldBeTrue();
        item.QuantityOwned.ShouldBe(2);
        item.QuantityDuplicate.ShouldBe(1);
    }

    [Fact]
    public async Task GetAlbumAsync_SoftDeletedCollection_TreatsAsNotOwnedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = StickerEntityMock.ValidWithTeam(team);
        var deletedCollection = new Domain.Entities.UserCollection
        {
            UserId            = userId,
            StickerId         = sticker.Id,
            QuantityOwned     = 1,
            QuantityDuplicate = 0,
            DeletedAt         = DateTime.UtcNow.AddDays(-1)
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.UserCollections.AddAsync(deletedCollection);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Items[0].Owned.ShouldBeFalse();
        result.Value.Items[0].QuantityOwned.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_Pagination_ReturnsCorrectPageAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickers = Enumerable.Range(1, 5).Select(i => new Domain.Entities.Sticker
        {
            Code       = $"BRA{i:000}",
            PlayerName = $"Player {i}",
            Rarity     = StickerRarity.Base,
            TeamId     = team.Id
        }).ToList();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickers);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, page: 1, pageSize: 2);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Items.Count.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(5);
        result.Value.Page.ShouldBe(1);
        result.Value.PageSize.ShouldBe(2);
        result.Value.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task GetAlbumAsync_Page2_ReturnsCorrectItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var stickers = Enumerable.Range(1, 4).Select(i => new Domain.Entities.Sticker
        {
            Code       = $"BRA{i:000}",
            PlayerName = $"Player {i}",
            Rarity     = StickerRarity.Base,
            TeamId     = team.Id
        }).ToList();

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddRangeAsync(stickers);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var queryPage1 = DefaultQuery(userId, page: 1, pageSize: 2);
        var queryPage2 = DefaultQuery(userId, page: 2, pageSize: 2);

        // Act
        var resultPage1 = await repository.GetAlbumAsync(queryPage1, CancellationToken.None);
        var resultPage2 = await repository.GetAlbumAsync(queryPage2, CancellationToken.None);

        // Assert
        resultPage1.IsSuccess.ShouldBeTrue();
        resultPage2.IsSuccess.ShouldBeTrue();
        resultPage1.Value!.Items.Count.ShouldBe(2);
        resultPage2.Value!.Items.Count.ShouldBe(2);
        var page1Codes = resultPage1.Value.Items.Select(x => x.Code).ToList();
        var page2Codes = resultPage2.Value.Items.Select(x => x.Code).ToList();
        page1Codes.Intersect(page2Codes).ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAlbumAsync_PageBeyondLastPage_ReturnsEmptyItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamEntityMock.Valid();
        var sticker = StickerEntityMock.ValidWithTeam(team);

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, page: 99, pageSize: 20);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetAlbumAsync_SortByTeamFalse_OrdersByCodeAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil   = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickerBra = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerArg = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi",     Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBra, stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, sortByTeam: false);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Items.Count.ShouldBe(2);
        result.Value.Items[0].Code.ShouldBe("ARG001");
        result.Value.Items[1].Code.ShouldBe("BRA001");
    }

    [Fact]
    public async Task GetAlbumAsync_SortByTeamTrue_OrdersByTeamNameAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil    = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickerBra = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerArg = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi",     Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBra, stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, sortByTeam: true);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Items.Count.ShouldBe(2);
        result.Value.Items[0].TeamName.ShouldBe("Argentina");
        result.Value.Items[1].TeamName.ShouldBe("Brasil");
    }

    [Fact]
    public async Task GetAlbumAsync_ResponseContainsCorrectFields_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = new Domain.Entities.Team { Name = "Brasil", Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var sticker = new Domain.Entities.Sticker
        {
            Code       = "BRA001",
            PlayerName = "Marquinhos",
            Rarity     = StickerRarity.Foil,
            TeamId     = team.Id
        };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var item = result.Value!.Items[0];
        item.StickerId.ShouldBe(sticker.Id);
        item.Code.ShouldBe("BRA001");
        item.PlayerName.ShouldBe("Marquinhos");
        item.TeamName.ShouldBe("Brasil");
        item.TeamCode.ShouldBe("BRA");
        item.Rarity.ShouldBe(StickerRarity.Foil);
        item.Owned.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAlbumAsync_FilterByTeamId_ReturnsOnlyStickersOfThatTeamAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil    = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickerBra1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr",  Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerBra2 = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerArg  = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi",       Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBra1, stickerBra2, stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, teamId: teamBrazil.Id);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(2);
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Items.ShouldAllBe(x => x.TeamName == "Brasil");
    }

    [Fact]
    public async Task GetAlbumAsync_FilterByTeamId_ExcludesStickersOfOtherTeamsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil    = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickerBra = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerArg = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi",     Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBra, stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, teamId: teamArgentina.Id);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(1);
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items[0].Code.ShouldBe("ARG001");
        result.Value.Items[0].TeamName.ShouldBe("Argentina");
    }

    [Fact]
    public async Task GetAlbumAsync_FilterByTeamIdWithPagination_ReturnsCorrectPageAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil    = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickersBra = Enumerable.Range(1, 5).Select(i => new Domain.Entities.Sticker
        {
            Code       = $"BRA{i:000}",
            PlayerName = $"Player BRA {i}",
            Rarity     = StickerRarity.Base,
            TeamId     = teamBrazil.Id
        }).ToList();

        var stickerArg = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi", Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickersBra);
        await dbContext.Stickers.AddAsync(stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, page: 1, pageSize: 2, teamId: teamBrazil.Id);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(5);
        result.Value.Items.Count.ShouldBe(2);
        result.Value.TotalPages.ShouldBe(3);
        result.Value.Items.ShouldAllBe(x => x.TeamName == "Brasil");
    }

    [Fact]
    public async Task GetAlbumAsync_FilterByTeamIdWithSortByTeam_ReturnsOnlyThatTeamOrderedByCodeAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil    = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickerBra2 = new Domain.Entities.Sticker { Code = "BRA002", PlayerName = "Vinicius Jr", Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerBra1 = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr",   Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerArg  = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi",       Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBra2, stickerBra1, stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, sortByTeam: true, teamId: teamBrazil.Id);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(2);
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Items[0].Code.ShouldBe("BRA001");
        result.Value.Items[1].Code.ShouldBe("BRA002");
    }

    [Fact]
    public async Task GetAlbumAsync_WithoutTeamFilter_ReturnsAllTeamsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamBrazil    = new Domain.Entities.Team { Name = "Brasil",    Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var teamArgentina = new Domain.Entities.Team { Name = "Argentina", Code = "ARG", FlagUrl = "https://flags.example.com/arg.png" };

        var stickerBra = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = teamBrazil.Id };
        var stickerArg = new Domain.Entities.Sticker { Code = "ARG001", PlayerName = "Messi",     Rarity = StickerRarity.Base, TeamId = teamArgentina.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddRangeAsync(teamBrazil, teamArgentina);
        await dbContext.Stickers.AddRangeAsync(stickerBra, stickerArg);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, teamId: null);

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(2);
        result.Value.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetAlbumAsync_FilterByNonExistentTeamId_ReturnsEmptyAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team    = new Domain.Entities.Team { Name = "Brasil", Code = "BRA", FlagUrl = "https://flags.example.com/bra.png" };
        var sticker = new Domain.Entities.Sticker { Code = "BRA001", PlayerName = "Neymar Jr", Rarity = StickerRarity.Base, TeamId = team.Id };

        await using var dbContext = CreateDbContext();
        await dbContext.Teams.AddAsync(team);
        await dbContext.Stickers.AddAsync(sticker);
        await dbContext.SaveChangesAsync();

        var repository = new StickerRepository(dbContext);
        var query = DefaultQuery(userId, teamId: Guid.NewGuid());

        // Act
        var result = await repository.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TotalCount.ShouldBe(0);
        result.Value.Items.ShouldBeEmpty();
    }
}
