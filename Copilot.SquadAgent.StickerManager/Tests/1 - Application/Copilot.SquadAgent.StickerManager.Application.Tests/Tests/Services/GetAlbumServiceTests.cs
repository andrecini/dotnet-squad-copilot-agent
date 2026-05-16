using AutoMapper;
using Copilot.SquadAgent.StickerManager.Application.Services.Collection;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;
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

    [Fact]
    public async Task GetAlbumAsync_WithMixedCollection_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var album = AlbumStickerModelMock.List(ownedCount: 2, missingCount: 3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Success(album))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(5);
    }

    [Fact]
    public async Task GetAlbumAsync_OwnedStickersHaveOwnedFlagTrue_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var album = AlbumStickerModelMock.List(ownedCount: 2, missingCount: 1);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Success(album))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count(x => x.Owned).ShouldBe(2);
        result.Value!.Count(x => !x.Owned).ShouldBe(1);
    }

    [Fact]
    public async Task GetAlbumAsync_WithEmptyAlbum_ReturnsEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var album = AlbumStickerModelMock.Empty();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Success(album))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAlbumAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetAlbumAsync_ValidUserId_CallsRepositoryOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var album = AlbumStickerModelMock.List();

        var stickerRepositoryMock = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Success(album));

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepositoryMock.Build(), userCollectionRepository, _mapper);

        // Act
        await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        stickerRepositoryMock.VerifyGetAlbumAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetAlbumAsync_OwnedStickerHasCorrectQuantities_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ownedSticker = AlbumStickerModelMock.OwnedSticker();
        var album = new List<AlbumStickerModel> { ownedSticker };

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Success(album))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var item = result.Value!.First();
        item.Owned.ShouldBeTrue();
        item.QuantityOwned.ShouldBe(1);
        item.QuantityDuplicate.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_MissingStickerHasOwnedFalse_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var missingSticker = AlbumStickerModelMock.MissingSticker();
        var album = new List<AlbumStickerModel> { missingSticker };

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>>.Success(album))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetAlbumAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var item = result.Value!.First();
        item.Owned.ShouldBeFalse();
        item.QuantityOwned.ShouldBe(0);
        item.QuantityDuplicate.ShouldBe(0);
    }
}
