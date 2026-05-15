using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class ListCollectionAppServiceTests
{
    private readonly IMapper _mapper;

    public ListCollectionAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task ListCollectionAsync_WithoutFilters_ReturnsOkWithItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var items = CollectionItemModelMock.List(3);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<CollectionItemResponse>>>();
        var ok = (Ok<IReadOnlyList<CollectionItemResponse>>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ListCollectionAsync_WithTeamFilter_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.WithTeamFilter(teamId);
        var items = CollectionItemModelMock.List(2);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<CollectionItemResponse>>>();
        var ok = (Ok<IReadOnlyList<CollectionItemResponse>>)result;
        ok.Value!.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListCollectionAsync_WithRarityFilter_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.WithRarityFilter();
        var items = CollectionItemModelMock.List(1);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<CollectionItemResponse>>>();
        var ok = (Ok<IReadOnlyList<CollectionItemResponse>>)result;
        ok.Value!.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ListCollectionAsync_WithSort_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.WithSort("player_name");
        var items = CollectionItemModelMock.List(4);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<CollectionItemResponse>>>();
        var ok = (Ok<IReadOnlyList<CollectionItemResponse>>)result;
        ok.Value!.Count.ShouldBe(4);
    }

    [Fact]
    public async Task ListCollectionAsync_EmptyCollection_ReturnsOkWithEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var items = CollectionItemModelMock.List(0);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<CollectionItemResponse>>>();
        var ok = (Ok<IReadOnlyList<CollectionItemResponse>>)result;
        ok.Value!.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListCollectionAsync_ServiceReturnsInternalError_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ListCollectionAsync_ValidRequest_CallsServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var items = CollectionItemModelMock.List(2);

        var collectionServiceMock = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyListCollectionAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task ListCollectionAsync_ResponseContainsCorrectTeamName_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var items = new List<CollectionItemModel>
        {
            new()
            {
                StickerId         = Guid.NewGuid(),
                PlayerName        = "Vinicius Jr",
                TeamName          = "Brasil",
                TeamCode          = "BRA",
                Rarity            = Domain.Enums.StickerRarity.Base,
                QuantityOwned     = 2,
                QuantityDuplicate = 1,
                AcquiredAt        = DateTime.UtcNow
            }
        };

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<CollectionItemResponse>>>();
        var ok = (Ok<IReadOnlyList<CollectionItemResponse>>)result;
        ok.Value.ShouldNotBeNull();
        var first = ok.Value!.First();
        first.Team.ShouldBe("Brasil");
        first.PlayerName.ShouldBe("Vinicius Jr");
        first.QuantityOwned.ShouldBe(2);
        first.QuantityDuplicate.ShouldBe(1);
    }
}
