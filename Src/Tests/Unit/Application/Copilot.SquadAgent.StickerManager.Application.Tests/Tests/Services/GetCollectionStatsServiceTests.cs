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

public class GetCollectionStatsServiceTests
{
    private readonly IMapper _mapper;

    public GetCollectionStatsServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task GetCollectionStatsAsync_WithPopulatedCollection_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetStatsAsync(Result<CollectionStatsModel>.Success(stats))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalOwned.ShouldBe(100);
        result.Value.TotalMissing.ShouldBe(550);
        result.Value.TotalStickers.ShouldBe(650);
        result.Value.Duplicates.ShouldBe(8);
    }

    [Fact]
    public async Task GetCollectionStatsAsync_WithEmptyCollection_ReturnsSuccessWithZerosAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Empty();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetStatsAsync(Result<CollectionStatsModel>.Success(stats))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TotalOwned.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0);
        result.Value.ByTeam.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCollectionStatsAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetStatsAsync(Result<CollectionStatsModel>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetCollectionStatsAsync_ValidUserId_CallsRepositoryOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepositoryMock = new UserCollectionRepositoryMock()
            .SetupGetStatsAsync(Result<CollectionStatsModel>.Success(stats));

        var service = new CollectionService(stickerRepository, userCollectionRepositoryMock.Build(), _mapper);

        // Act
        await service.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        userCollectionRepositoryMock.VerifyGetStatsAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetCollectionStatsAsync_ByTeamContainsExpectedData_ReturnsSuccessAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stats = CollectionStatsModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetStatsAsync(Result<CollectionStatsModel>.Success(stats))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetCollectionStatsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.ByTeam.ShouldNotBeEmpty();
        result.Value.ByTeam.First().TeamName.ShouldBe("Brasil");
        result.Value.ByTeam.First().CompletionPercentage.ShouldBe(75.0);
    }
}
