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

public class ListCollectionServiceTests
{
    private readonly IMapper _mapper;

    public ListCollectionServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task ListCollectionAsync_WithoutFilters_ReturnsSuccessWithItemsAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.Valid();
        var paged = CollectionItemModelMock.Paged(3);

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ListCollectionAsync_WithTeamFilter_ReturnsSuccessAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var model = ListCollectionModelMock.WithTeamFilter(teamId: teamId);
        var paged = CollectionItemModelMock.Paged(2);

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListCollectionAsync_WithRarityFilter_ReturnsSuccessAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.WithRarityFilter();
        var paged = CollectionItemModelMock.Paged(1);

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ListCollectionAsync_WithPagination_ReturnsSuccessAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.WithPagination(page: 2, limit: 10);
        var paged = CollectionItemModelMock.Paged(10);

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(10);
    }

    [Fact]
    public async Task ListCollectionAsync_WithSort_ReturnsSuccessAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.WithSort(sort: "player_name");
        var paged = CollectionItemModelMock.Paged(5);

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(5);
    }

    [Fact]
    public async Task ListCollectionAsync_EmptyCollection_ReturnsSuccessWithEmptyListAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.Valid();
        var paged = CollectionItemModelMock.PagedEmpty();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListCollectionAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ListCollectionAsync_ValidModel_CallsRepositoryOnceAsync()
    {
        // Arrange
        var model = ListCollectionModelMock.Valid();
        var paged = CollectionItemModelMock.Paged(2);

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepositoryMock = new UserCollectionRepositoryMock()
            .SetupListByUserAsync(Result<PagedResult<CollectionItemModel>>.Success(paged));

        var service = new CollectionService(stickerRepository, userCollectionRepositoryMock.Build(), _mapper);

        // Act
        await service.ListCollectionAsync(model, CancellationToken.None);

        // Assert
        userCollectionRepositoryMock.VerifyListByUserAsyncCalled(Times.Once());
    }
}
