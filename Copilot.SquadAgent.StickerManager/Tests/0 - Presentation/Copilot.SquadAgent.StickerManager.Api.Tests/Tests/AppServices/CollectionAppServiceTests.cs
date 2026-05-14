using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class CollectionAppServiceTests
{
    private readonly IMapper _mapper;

    public CollectionAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task AddStickerAsync_ValidRequest_ReturnsCreatedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = AddToCollectionRequestMock.Valid();
        var userCollectionModel = UserCollectionModelMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupAddStickerAsync(Result<Domain.Models.Collection.UserCollectionModel>.Success(userCollectionModel))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.AddStickerAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Created<DTOs.Responses.AddToCollectionResponse>>();
        var created = (Created<DTOs.Responses.AddToCollectionResponse>)result;
        created.Value.ShouldNotBeNull();
        created.Value!.CollectionId.ShouldBe(userCollectionModel.Id);
    }

    [Fact]
    public async Task AddStickerAsync_ValidRequest_ReturnsCorrectLocationHeaderAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = AddToCollectionRequestMock.Valid();
        var userCollectionModel = UserCollectionModelMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupAddStickerAsync(Result<Domain.Models.Collection.UserCollectionModel>.Success(userCollectionModel))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.AddStickerAsync(userId, request, CancellationToken.None);

        // Assert
        var created = (Created<DTOs.Responses.AddToCollectionResponse>)result;
        created.Location.ShouldBe($"/api/v1/collection/{userCollectionModel.Id}");
    }

    [Fact]
    public async Task AddStickerAsync_ServiceReturnsNotFound_ReturnsProblemAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = AddToCollectionRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupAddStickerAsync(Result<Domain.Models.Collection.UserCollectionModel>.Failure(ResultCode.NotFound, "Figurinha não encontrada.", 404))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.AddStickerAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task AddStickerAsync_ServiceReturnsBusinessError_ReturnsProblemAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = AddToCollectionRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupAddStickerAsync(Result<Domain.Models.Collection.UserCollectionModel>.Failure(ResultCode.BusinessError, "Limite máximo de 10 duplicatas atingido para esta figurinha.", 422))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.AddStickerAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(422);
    }

    [Fact]
    public async Task AddStickerAsync_ServiceReturnsFailureWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = AddToCollectionRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupAddStickerAsync(Result<Domain.Models.Collection.UserCollectionModel>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.AddStickerAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task AddStickerAsync_ValidRequest_CallsCollectionServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = AddToCollectionRequestMock.Valid();
        var userCollectionModel = UserCollectionModelMock.Valid();

        var collectionServiceMock = new CollectionServiceMock()
            .SetupAddStickerAsync(Result<Domain.Models.Collection.UserCollectionModel>.Success(userCollectionModel));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.AddStickerAsync(userId, request, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyAddStickerAsyncCalled(Moq.Times.Once());
    }
}
