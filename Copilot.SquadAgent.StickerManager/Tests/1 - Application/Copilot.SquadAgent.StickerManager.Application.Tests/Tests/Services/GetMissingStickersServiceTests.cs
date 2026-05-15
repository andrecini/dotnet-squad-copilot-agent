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

public class GetMissingStickersServiceTests
{
    private readonly IMapper _mapper;

    public GetMissingStickersServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task ListMissingStickersAsync_UserHasMissingStickers_ReturnsSuccessWithItemsAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();
        var items = MissingStickerItemModelMock.List(5);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(5);
    }

    [Fact]
    public async Task ListMissingStickersAsync_UserHasAllStickers_ReturnsSuccessWithEmptyListAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();
        var items = MissingStickerItemModelMock.List(0);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithSortByTeam_ReturnsSuccessAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.WithTeamSort();
        var items = MissingStickerItemModelMock.List(3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithSortByNumber_ReturnsSuccessAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.WithNumberSort();
        var items = MissingStickerItemModelMock.List(4);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count.ShouldBe(4);
    }

    [Fact]
    public async Task ListMissingStickersAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
        result.Message.ShouldBe("Erro de banco.");
    }

    [Fact]
    public async Task ListMissingStickersAsync_ValidModel_CallsRepositoryOnceAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();
        var items = MissingStickerItemModelMock.List(2);

        var stickerRepositoryMock = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items));

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepositoryMock.Build(), userCollectionRepository, _mapper);

        // Act
        await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        stickerRepositoryMock.VerifyListMissingByUserAsyncCalled(Times.Once());
    }
}
