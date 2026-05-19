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

public class GetAlbumAppServiceTests
{
    private readonly IMapper _mapper;

    public GetAlbumAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task GetAlbumAsync_WithDefaultQuery_ReturnsOkWithPagedResponseAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();
        var paged = AlbumStickerModelMock.Paged(ownedCount: 2, missingCount: 3);

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Items.Count.ShouldBe(5);
        ok.Value.TotalCount.ShouldBe(5);
        ok.Value.Page.ShouldBe(1);
        ok.Value.PageSize.ShouldBe(20);
    }

    [Fact]
    public async Task GetAlbumAsync_WithEmptyAlbum_ReturnsOkWithEmptyItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();
        var paged = AlbumStickerModelMock.PagedEmpty();

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value!.Items.ShouldBeEmpty();
        ok.Value.TotalCount.ShouldBe(0);
        ok.Value.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task GetAlbumAsync_WithPagination_ReturnsCorrectPaginationMetadataAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.WithPagination(page: 3, pageSize: 10);
        var paged = new PagedResult<AlbumStickerModel>
        {
            Items      = AlbumStickerModelMock.Paged(ownedCount: 5, missingCount: 5).Items,
            TotalCount = 100,
            Page       = 3,
            PageSize   = 10
        };

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value!.Page.ShouldBe(3);
        ok.Value.PageSize.ShouldBe(10);
        ok.Value.TotalCount.ShouldBe(100);
        ok.Value.TotalPages.ShouldBe(10);
    }

    [Fact]
    public async Task GetAlbumAsync_SortByTeamTrue_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.WithSortByTeam();
        var paged = AlbumStickerModelMock.Paged();

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
    }

    [Fact]
    public async Task GetAlbumAsync_ServiceReturnsFailure_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Failure(ResultCode.InternalError, "Erro interno.", 500))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetAlbumAsync_ServiceReturnsFailureWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetAlbumAsync_ValidRequest_CallsServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();
        var paged = AlbumStickerModelMock.Paged();

        var collectionServiceMock = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyGetAlbumAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetAlbumAsync_WithTeamFilter_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.WithTeamFilter(teamId);
        var paged = AlbumStickerModelMock.Paged(ownedCount: 1, missingCount: 1);

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Items.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetAlbumAsync_WithTeamFilterAndPagination_ReturnsCorrectPaginationMetadataAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.WithTeamFilterAndPagination(teamId, page: 2, pageSize: 5);
        var paged = new PagedResult<AlbumStickerModel>
        {
            Items      = AlbumStickerModelMock.Paged(ownedCount: 2, missingCount: 3).Items,
            TotalCount = 15,
            Page       = 2,
            PageSize   = 5
        };

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value!.Page.ShouldBe(2);
        ok.Value.PageSize.ShouldBe(5);
        ok.Value.TotalCount.ShouldBe(15);
        ok.Value.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task GetAlbumAsync_WithTeamFilterAndSort_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.WithTeamFilterAndSort(teamId);
        var paged = AlbumStickerModelMock.Paged(ownedCount: 1, missingCount: 2);

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GetAlbumAsync_WithoutTeamFilter_ReturnsAllItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();
        var paged = AlbumStickerModelMock.Paged(ownedCount: 3, missingCount: 7);

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        ok.Value!.Items.Count.ShouldBe(10);
        ok.Value.TotalCount.ShouldBe(10);
    }

    [Fact]
    public async Task GetAlbumAsync_TeamIdIsMappedToModel_CallsServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.WithTeamFilter(teamId);
        var paged = AlbumStickerModelMock.Paged();

        var collectionServiceMock = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyGetAlbumAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetAlbumAsync_ResponseItemsHaveCorrectTeamName_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = AlbumQueryRequestMock.Default();
        var paged = new PagedResult<AlbumStickerModel>
        {
            Items = new List<AlbumStickerModel>
            {
                new()
                {
                    StickerId         = Guid.NewGuid(),
                    Code              = "BRA-001",
                    PlayerName        = "Neymar Jr",
                    TeamName          = "Brasil",
                    TeamCode          = "BRA",
                    Rarity            = Domain.Enums.StickerRarity.Base,
                    Owned             = true,
                    QuantityOwned     = 1,
                    QuantityDuplicate = 0
                }
            },
            TotalCount = 1,
            Page       = 1,
            PageSize   = 20
        };

        var collectionService = new CollectionServiceMock()
            .SetupGetAlbumAsync(Result<PagedResult<AlbumStickerModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetAlbumAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedAlbumResponse>>();
        var ok = (Ok<PagedAlbumResponse>)result;
        var first = ok.Value!.Items.First();
        first.Team.ShouldBe("Brasil");
        first.PlayerName.ShouldBe("Neymar Jr");
        first.Owned.ShouldBeTrue();
    }
}
