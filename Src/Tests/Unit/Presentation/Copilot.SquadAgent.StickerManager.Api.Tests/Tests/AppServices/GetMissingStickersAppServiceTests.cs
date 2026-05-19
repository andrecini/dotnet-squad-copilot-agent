using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class GetMissingStickersAppServiceTests
{
    private readonly IMapper _mapper;

    public GetMissingStickersAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task ListMissingStickersAsync_UserHasMissingStickers_ReturnsOkWithItemsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();
        var paged = MissingStickerItemModelMock.Paged(5);

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedMissingStickersResponse>>();
        var ok = (Ok<PagedMissingStickersResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Items.Count.ShouldBe(5);
        ok.Value.TotalCount.ShouldBe(5);
        ok.Value.Page.ShouldBe(1);
        ok.Value.PageSize.ShouldBe(20);
    }

    [Fact]
    public async Task ListMissingStickersAsync_UserHasAllStickers_ReturnsOkWithEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();
        var paged = MissingStickerItemModelMock.PagedEmpty();

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedMissingStickersResponse>>();
        var ok = (Ok<PagedMissingStickersResponse>)result;
        ok.Value!.Items.Count.ShouldBe(0);
        ok.Value.TotalCount.ShouldBe(0);
        ok.Value.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithSortParam_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.WithSort("team");
        var paged = MissingStickerItemModelMock.Paged(3);

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedMissingStickersResponse>>();
        var ok = (Ok<PagedMissingStickersResponse>)result;
        ok.Value!.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithPagination_ReturnsCorrectPaginationMetadataAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.WithPagination(page: 2, pageSize: 10);
        var paged = new PagedResult<MissingStickerItemModel>
        {
            Items      = MissingStickerItemModelMock.List(10),
            TotalCount = 50,
            Page       = 2,
            PageSize   = 10
        };

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedMissingStickersResponse>>();
        var ok = (Ok<PagedMissingStickersResponse>)result;
        ok.Value!.Items.Count.ShouldBe(10);
        ok.Value.Page.ShouldBe(2);
        ok.Value.PageSize.ShouldBe(10);
        ok.Value.TotalCount.ShouldBe(50);
        ok.Value.TotalPages.ShouldBe(5);
    }

    [Fact]
    public async Task ListMissingStickersAsync_ServiceReturnsFailure_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Failure(ResultCode.InternalError, "Erro interno.", 500))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ListMissingStickersAsync_ResponseContainsCorrectTeamName_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();
        var paged = new PagedResult<MissingStickerItemModel>
        {
            Items = new List<MissingStickerItemModel>
            {
                new()
                {
                    StickerId  = Guid.NewGuid(),
                    Code       = "BRA001",
                    PlayerName = "Richarlison",
                    TeamName   = "Brasil",
                    TeamCode   = "BRA",
                    Rarity     = StickerRarity.Foil
                }
            },
            TotalCount = 1,
            Page       = 1,
            PageSize   = 20
        };

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<PagedMissingStickersResponse>>();
        var ok = (Ok<PagedMissingStickersResponse>)result;
        ok.Value.ShouldNotBeNull();
        var first = ok.Value!.Items.First();
        first.Team.ShouldBe("Brasil");
        first.PlayerName.ShouldBe("Richarlison");
        first.Code.ShouldBe("BRA001");
        first.Rarity.ShouldBe(StickerRarity.Foil);
    }

    [Fact]
    public async Task ListMissingStickersAsync_ValidRequest_CallsServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();
        var paged = MissingStickerItemModelMock.Paged(2);

        var collectionServiceMock = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<PagedResult<MissingStickerItemModel>>.Success(paged));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyListMissingStickersAsyncCalled(Times.Once());
    }
}
