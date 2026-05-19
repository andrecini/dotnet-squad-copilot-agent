using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class GetCollectionStatsAppServiceTests
{
    private readonly IMapper _mapper;

    public GetCollectionStatsAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task GetCollectionStatsAsync_WithPopulatedCollection_ReturnsOkWithStatsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupGetCollectionStatsAsync(Result<CollectionStatsModel>.Success(stats))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<CollectionStatsResponse>>();
        var ok = (Ok<CollectionStatsResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.TotalOwned.ShouldBe(100);
        ok.Value.TotalMissing.ShouldBe(550);
        ok.Value.TotalStickers.ShouldBe(650);
        ok.Value.Duplicates.ShouldBe(8);
    }

    [Fact]
    public async Task GetCollectionStatsAsync_WithEmptyCollection_ReturnsOkWithZeroStatsAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Empty();

        var collectionService = new CollectionServiceMock()
            .SetupGetCollectionStatsAsync(Result<CollectionStatsModel>.Success(stats))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<CollectionStatsResponse>>();
        var ok = (Ok<CollectionStatsResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.TotalOwned.ShouldBe(0);
        ok.Value.CompletionPercentage.ShouldBe(0);
        ok.Value.ByTeam.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCollectionStatsAsync_ByTeamContainsCorrectData_ReturnsOkAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupGetCollectionStatsAsync(Result<CollectionStatsModel>.Success(stats))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<CollectionStatsResponse>>();
        var ok = (Ok<CollectionStatsResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.ByTeam.Count.ShouldBe(2);
        ok.Value.ByTeam.First().Team.ShouldBe("Brasil");
        ok.Value.ByTeam.First().Owned.ShouldBe(15);
        ok.Value.ByTeam.First().CompletionPercentage.ShouldBe(75.0);
    }

    [Fact]
    public async Task GetCollectionStatsAsync_ServiceReturnsInternalError_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var collectionService = new CollectionServiceMock()
            .SetupGetCollectionStatsAsync(Result<CollectionStatsModel>.Failure(ResultCode.InternalError, "Erro interno.", 500))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetCollectionStatsAsync_ValidUserId_CallsServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Valid();

        var collectionServiceMock = new CollectionServiceMock()
            .SetupGetCollectionStatsAsync(Result<CollectionStatsModel>.Success(stats));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyGetCollectionStatsAsyncCalled(Times.Once());
    }
}
