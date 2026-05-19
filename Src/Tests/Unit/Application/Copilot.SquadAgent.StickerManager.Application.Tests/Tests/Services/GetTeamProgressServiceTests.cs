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

public class GetTeamProgressServiceTests
{
    private readonly IMapper _mapper;

    public GetTeamProgressServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task GetTeamProgressAsync_WithPartialCollection_ReturnsSuccessAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var progress = TeamProgressModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Success(progress))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.TeamName.ShouldBe("Brasil");
        result.Value.TotalStickers.ShouldBe(20);
        result.Value.OwnedCount.ShouldBe(15);
        result.Value.CompletionPercentage.ShouldBe(75.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_WithNoStickers_ReturnsZeroCountsAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var progress = TeamProgressModelMock.WithNoStickers();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Success(progress))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.OwnedCount.ShouldBe(0);
        result.Value.CompletionPercentage.ShouldBe(0.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_WithCompleteCollection_Returns100PercentAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var progress = TeamProgressModelMock.Complete();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Success(progress))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.OwnedCount.ShouldBe(20);
        result.Value.TotalStickers.ShouldBe(20);
        result.Value.CompletionPercentage.ShouldBe(100.0);
    }

    [Fact]
    public async Task GetTeamProgressAsync_TeamNotFound_ReturnsNotFoundAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Failure(ResultCode.NotFound, "Time não encontrado.", 404))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetTeamProgressAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task GetTeamProgressAsync_ValidInput_CallsRepositoryOnceAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var progress = TeamProgressModelMock.Valid();

        var stickerRepositoryMock = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Success(progress));

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepositoryMock.Build(), userCollectionRepository, _mapper);

        // Act
        await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        stickerRepositoryMock.VerifyGetTeamProgressAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task GetTeamProgressAsync_ResultContainsCorrectTeamId_ReturnsSuccessAsync()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var progress = TeamProgressModelMock.Valid();
        progress.TeamId = teamId;

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetTeamProgressAsync(Result<TeamProgressModel>.Success(progress))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();
        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.GetTeamProgressAsync(teamId, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.TeamId.ShouldBe(teamId);
    }
}
