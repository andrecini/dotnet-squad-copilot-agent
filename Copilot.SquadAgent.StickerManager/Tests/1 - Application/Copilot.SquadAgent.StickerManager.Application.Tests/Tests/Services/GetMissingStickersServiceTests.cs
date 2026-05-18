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
        var paged = MissingStickerItemModelMock.Paged(5);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(5);
    }

    [Fact]
    public async Task ListMissingStickersAsync_UserHasAllStickers_ReturnsSuccessWithEmptyListAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();
        var paged = MissingStickerItemModelMock.PagedEmpty();

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithSortByTeam_ReturnsSuccessAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.WithTeamSort();
        var paged = MissingStickerItemModelMock.Paged(3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithSortByNumber_ReturnsSuccessAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.WithNumberSort();
        var paged = MissingStickerItemModelMock.Paged(4);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(4);
    }

    [Fact]
    public async Task ListMissingStickersAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
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
    public async Task ListMissingStickersAsync_WithPagination_ReturnsSuccessAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.WithPagination(page: 2, limit: 10);
        var paged = MissingStickerItemModelMock.Paged(10);

        var stickerRepository = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(10);
    }

    [Fact]
    public async Task ListMissingStickersAsync_ValidModel_CallsRepositoryOnceAsync()
    {
        // Arrange
        var model = MissingStickersModelMock.Valid();
        var paged = MissingStickerItemModelMock.Paged(2);

        var stickerRepositoryMock = new StickerRepositoryMock()
            .SetupListMissingByUserAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged));

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepositoryMock.Build(), userCollectionRepository, _mapper);

        // Act
        await service.ListMissingStickersAsync(model, CancellationToken.None);

        // Assert
        stickerRepositoryMock.VerifyListMissingByUserAsyncCalled(Times.Once());
    }
}
