using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
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
        var items = MissingStickerItemModelMock.List(5);

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<MissingStickerItemResponse>>>();
        var ok = (Ok<IReadOnlyList<MissingStickerItemResponse>>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Count.ShouldBe(5);
    }

    [Fact]
    public async Task ListMissingStickersAsync_UserHasAllStickers_ReturnsOkWithEmptyListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();
        var items = MissingStickerItemModelMock.List(0);

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<MissingStickerItemResponse>>>();
        var ok = (Ok<IReadOnlyList<MissingStickerItemResponse>>)result;
        ok.Value!.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithSortParam_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.WithSort("team");
        var items = MissingStickerItemModelMock.List(3);

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<MissingStickerItemResponse>>>();
        var ok = (Ok<IReadOnlyList<MissingStickerItemResponse>>)result;
        ok.Value!.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ListMissingStickersAsync_WithPagination_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.WithPagination(page: 2, pageSize: 10);
        var items = MissingStickerItemModelMock.List(10);

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<MissingStickerItemResponse>>>();
        var ok = (Ok<IReadOnlyList<MissingStickerItemResponse>>)result;
        ok.Value!.Count.ShouldBe(10);
    }

    [Fact]
    public async Task ListMissingStickersAsync_ServiceReturnsFailure_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = MissingStickersQueryRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Failure(ResultCode.InternalError, "Erro interno.", 500))
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
        var items = new List<MissingStickerItemModel>
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
        };

        var collectionService = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<IReadOnlyList<MissingStickerItemResponse>>>();
        var ok = (Ok<IReadOnlyList<MissingStickerItemResponse>>)result;
        ok.Value.ShouldNotBeNull();
        var first = ok.Value!.First();
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
        var items = MissingStickerItemModelMock.List(2);

        var collectionServiceMock = new CollectionServiceMock()
            .SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>>.Success(items));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.ListMissingStickersAsync(userId, query, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyListMissingStickersAsyncCalled(Times.Once());
    }
}
