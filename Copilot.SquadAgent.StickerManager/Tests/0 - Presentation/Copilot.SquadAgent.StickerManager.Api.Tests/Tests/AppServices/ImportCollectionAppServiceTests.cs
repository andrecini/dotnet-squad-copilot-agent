using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
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

public class ImportCollectionAppServiceTests
{
    private readonly IMapper _mapper;

    public ImportCollectionAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task ImportCollectionAsync_ValidCsvAndServiceSuccess_ReturnsOkWithResultAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = ImportCollectionRequestMock.Valid();
        var resultModel = ImportCollectionResultModelMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupImportCollectionAsync(Result<ImportCollectionResultModel>.Success(resultModel))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ImportCollectionAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<DTOs.Responses.ImportCollectionResponse>>();
        var ok = (Ok<DTOs.Responses.ImportCollectionResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Imported.ShouldBe(resultModel.Imported);
        ok.Value.Failed.ShouldBe(resultModel.Failed);
    }

    [Fact]
    public async Task ImportCollectionAsync_ValidCsvWithErrors_ReturnsOkWithErrorListAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = ImportCollectionRequestMock.Valid();
        var resultModel = ImportCollectionResultModelMock.WithErrors();

        var collectionService = new CollectionServiceMock()
            .SetupImportCollectionAsync(Result<ImportCollectionResultModel>.Success(resultModel))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ImportCollectionAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<DTOs.Responses.ImportCollectionResponse>>();
        var ok = (Ok<DTOs.Responses.ImportCollectionResponse>)result;
        ok.Value!.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ImportCollectionAsync_ServiceReturnsBusinessError_ReturnsProblemAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = ImportCollectionRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupImportCollectionAsync(Result<ImportCollectionResultModel>.Failure(ResultCode.BusinessError, "O arquivo CSV não pode ter mais de 1000 linhas.", 422))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ImportCollectionAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(422);
    }

    [Fact]
    public async Task ImportCollectionAsync_ServiceReturnsInternalError_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = ImportCollectionRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupImportCollectionAsync(Result<ImportCollectionResultModel>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ImportCollectionAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ImportCollectionAsync_ValidCsv_CallsCollectionServiceOnceAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = ImportCollectionRequestMock.Valid();
        var resultModel = ImportCollectionResultModelMock.Valid();

        var collectionServiceMock = new CollectionServiceMock()
            .SetupImportCollectionAsync(Result<ImportCollectionResultModel>.Success(resultModel));

        var appService = new CollectionAppService(collectionServiceMock.Build(), _mapper);

        // Act
        await appService.ImportCollectionAsync(userId, request, CancellationToken.None);

        // Assert
        collectionServiceMock.VerifyImportCollectionAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task ImportCollectionAsync_ServiceReturnsBusinessErrorWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = ImportCollectionRequestMock.Valid();

        var collectionService = new CollectionServiceMock()
            .SetupImportCollectionAsync(Result<ImportCollectionResultModel>.Failure(ResultCode.BusinessError, "Erro de negócio sem status code."))
            .Build();

        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.ImportCollectionAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }
}
