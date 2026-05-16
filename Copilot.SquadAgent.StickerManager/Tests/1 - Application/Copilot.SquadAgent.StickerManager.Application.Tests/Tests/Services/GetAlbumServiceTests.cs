using AutoMapper;
using Copilot.SquadAgent.StickerManager.Application.Services.Collection;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Services;

public class GetAlbumServiceTests
{
    private readonly IMapper _mapper;

    public GetAlbumServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    private static AlbumQueryModel DefaultQuery(int page = 1, int pageSize = 20, bool sortByTeam = false) => new()
    {
        UserId     = Guid.NewGuid(),
        Page       = page,
        PageSize   = pageSize,
        SortByTeam = sortByTeam
    };

    [Fact]
    public async Task GetAlbumAsync_WithMixedCollection_ReturnsSuccessAsync()
    {
        // Arrange
        var query = DefaultQuery();
        var paged = AlbumStickerModelMock.Paged(ownedCount: 2, missingCount: 3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(5);
        result.Value.TotalCount.ShouldBe(5);
    }

    [Fact]
    public async Task GetAlbumAsync_OwnedStickersHaveOwnedFlagTrue_ReturnsSuccessAsync()
    {
        // Arrange
        var query = DefaultQuery();
        var paged = AlbumStickerModelMock.Paged(ownedCount: 2, missingCount: 1);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Items.Count(x => x.Owned).ShouldBe(2);
        result.Value!.Items.Count(x => !x.Owned).ShouldBe(1);
    }

    [Fact]
    public async Task GetAlbumAsync_WithEmptyAlbum_ReturnsEmptyPagedResultAsync()
    {
        // Arrange
        var query = DefaultQuery();
        var paged = AlbumStickerModelMock.PagedEmpty();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
        result.Value.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var query = DefaultQuery();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetAlbumAsync_ValidQuery_CallsRepositoryOnceAsync()
    {
        // Arrange
        var query = DefaultQuery();
        var paged = AlbumStickerModelMock.Paged();

        var stickerRepositoryMock = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged));

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepositoryMock.Build(), userCollectionRepository, _mapper);

        // Act
        await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        stickerRepositoryMock.VerifyGetAlbumAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetAlbumAsync_PaginationValues_ArePreservedInResultAsync()
    {
        // Arrange
        var query = DefaultQuery(page: 2, pageSize: 10);
        var paged = new PagedResult<AlbumStickerModel>
        {
            Items      = AlbumStickerModelMock.List(ownedCount: 2, missingCount: 3),
            TotalCount = 50,
            Page       = 2,
            PageSize   = 10
        };

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Page.ShouldBe(2);
        result.Value.PageSize.ShouldBe(10);
        result.Value.TotalCount.ShouldBe(50);
        result.Value.TotalPages.ShouldBe(5);
    }

    [Fact]
    public async Task GetAlbumAsync_SortByTeamTrue_PassesQueryToRepositoryAsync()
    {
        // Arrange
        var query = DefaultQuery(sortByTeam: true);
        var paged = AlbumStickerModelMock.Paged();

        var stickerRepositoryMock = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged));

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepositoryMock.Build(), userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        stickerRepositoryMock.VerifyGetAlbumAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetAlbumAsync_OwnedStickerHasCorrectQuantities_ReturnsSuccessAsync()
    {
        // Arrange
        var query = DefaultQuery();
        var ownedSticker = AlbumStickerModelMock.OwnedSticker();
        var paged = new PagedResult<AlbumStickerModel>
        {
            Items      = new List<AlbumStickerModel> { ownedSticker },
            TotalCount = 1,
            Page       = 1,
            PageSize   = 20
        };

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var item = result.Value!.Items.First();
        item.Owned.ShouldBeTrue();
        item.QuantityOwned.ShouldBe(1);
        item.QuantityDuplicate.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_MissingStickerHasOwnedFalse_ReturnsSuccessAsync()
    {
        // Arrange
        var query = DefaultQuery();
        var missingSticker = AlbumStickerModelMock.MissingSticker();
        var paged = new PagedResult<AlbumStickerModel>
        {
            Items      = new List<AlbumStickerModel> { missingSticker },
            TotalCount = 1,
            Page       = 1,
            PageSize   = 20
        };

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var item = result.Value!.Items.First();
        item.Owned.ShouldBeFalse();
        item.QuantityOwned.ShouldBe(0);
        item.QuantityDuplicate.ShouldBe(0);
    }
}
