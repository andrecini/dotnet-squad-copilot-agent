using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;
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
        var paged = CollectionItemModelMock.Paged(3);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Items.Count.ShouldBe(3);
        ok.Value.TotalCount.ShouldBe(3);
        ok.Value.Page.ShouldBe(1);
        ok.Value.PageSize.ShouldBe(20);
    }

    [Fact]
    public async Task ListCollectionAsync_WithTeamFilter_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.WithTeamFilter(teamId);
        var paged = CollectionItemModelMock.Paged(2);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value!.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ListCollectionAsync_WithRarityFilter_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.WithRarityFilter();
        var paged = CollectionItemModelMock.Paged(1);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value!.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ListCollectionAsync_WithSort_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.WithSort("player_name");
        var paged = CollectionItemModelMock.Paged(4);

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value!.Items.Count.ShouldBe(4);
    }

    [Fact]
    public async Task ListCollectionAsync_EmptyCollection_ReturnsOkWithEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var paged = CollectionItemModelMock.PagedEmpty();

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value!.Items.Count.ShouldBe(0);
        ok.Value.TotalCount.ShouldBe(0);
        ok.Value.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task ListCollectionAsync_ServiceReturnsInternalError_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Failure(ResultCode.InternalError, "Erro interno."))
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
        var paged = CollectionItemModelMock.Paged(2);

        var collectionServiceMock = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyListCollectionAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task ListCollectionAsync_WithPagination_ReturnsCorrectPaginationMetadataAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var paged = new PagedResult<CollectionItemModel>
        {
            Items      = CollectionItemModelMock.List(5),
            TotalCount = 50,
            Page       = 2,
            PageSize   = 10
        };

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value!.Page.ShouldBe(2);
        ok.Value.PageSize.ShouldBe(10);
        ok.Value.TotalCount.ShouldBe(50);
        ok.Value.TotalPages.ShouldBe(5);
    }

    [Fact]
    public async Task ListCollectionAsync_ResponseContainsCorrectTeamName_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = CollectionQueryRequestMock.Valid();
        var paged = new PagedResult<CollectionItemModel>
        {
            Items = new List<CollectionItemModel>
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
            },
            TotalCount = 1,
            Page       = 1,
            PageSize   = 20
        };

        var collectionService = new CollectionServiceMock()
            .SetupListCollectionAsync(Result<PagedResult<CollectionItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListCollectionAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedCollectionResponse>>();
        var ok = (Ok<PagedCollectionResponse>)result;
        ok.Value.ShouldNotBeNull();
        var first = ok.Value!.Items.First();
        first.Team.ShouldBe("Brasil");
        first.PlayerName.ShouldBe("Vinicius Jr");
        first.QuantityOwned.ShouldBe(2);
        first.QuantityDuplicate.ShouldBe(1);
    }
}
