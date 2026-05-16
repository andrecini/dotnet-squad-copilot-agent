using AutoMapper;
using Copilot.SquadAgent.StickerManager.Application.Services.Collection;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using Shouldly;
using Xunit;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;
using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Services;

public class ImportCollectionServiceTests
{
    private readonly IMapper _mapper;

    public ImportCollectionServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task ImportCollectionAsync_ExceedsMaxLines_ReturnsBusinessErrorAsync()
    {
        // Arrange
        var model = ImportCollectionModelMock.ExceedingMaxLines();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.BusinessError);
        result.StatusCode.ShouldBe(422);
        result.Message!.ShouldContain("1000");
    }

    [Fact]
    public async Task ImportCollectionAsync_EmptyRows_ReturnsSuccessWithZeroImportedAsync()
    {
        // Arrange
        var model = ImportCollectionModelMock.WithEmptyRows();

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Imported.ShouldBe(0);
        result.Value.Failed.ShouldBe(0);
    }

    [Fact]
    public async Task ImportCollectionAsync_ValidNewStickers_ReturnsSuccessWithImportedCountAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stickerEntity = StickerEntityMock.Valid();
        var model = ImportCollectionModelMock.WithSingleRow(userId, stickerEntity.Code, quantity: 1);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByCodeAsync(Result<StickerEntity?>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(null))
            .SetupBulkUpsertAsync(Result<int>.Success(1))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Imported.ShouldBe(1);
        result.Value.Failed.ShouldBe(0);
        result.Value.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task ImportCollectionAsync_StickerNotFound_AddsErrorAndSkipsLineAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = ImportCollectionModelMock.WithSingleRow(userId, "INVALID_CODE");

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByCodeAsync(Result<StickerEntity?>.Success(null))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Imported.ShouldBe(0);
        result.Value.Failed.ShouldBe(1);
        result.Value.Errors.ShouldHaveSingleItem();
        result.Value.Errors[0].Line.ShouldBe(2);
        result.Value.Errors[0].Reason.ShouldContain("INVALID_CODE");
    }

    [Fact]
    public async Task ImportCollectionAsync_EmptyStickerNumber_AddsValidationErrorAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new ImportCollectionModel
        {
            UserId = userId,
            Rows = [new CsvStickerRowModel { StickerNumber = "", Quantity = 1 }]
        };

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Failed.ShouldBe(1);
        result.Value.Errors[0].Reason.ShouldContain("sticker_number");
    }

    [Fact]
    public async Task ImportCollectionAsync_InvalidQuantity_AddsValidationErrorAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new ImportCollectionModel
        {
            UserId = userId,
            Rows = [new CsvStickerRowModel { StickerNumber = "BRA01", Quantity = 0 }]
        };

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Failed.ShouldBe(1);
        result.Value.Errors[0].Reason.ShouldContain("quantity");
    }

    [Fact]
    public async Task ImportCollectionAsync_ExistingSticker_IncrementsQuantityOwnedAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stickerEntity = StickerEntityMock.Valid();
        var existingEntry = UserCollectionEntityMock.WithUserId(userId);
        existingEntry.QuantityOwned = 2;

        var model = ImportCollectionModelMock.WithSingleRow(userId, stickerEntity.Code, quantity: 3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByCodeAsync(Result<StickerEntity?>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(existingEntry))
            .SetupBulkUpsertAsync(Result<int>.Success(1))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Imported.ShouldBe(1);
        existingEntry.QuantityOwned.ShouldBe(5);
    }

    [Fact]
    public async Task ImportCollectionAsync_BulkUpsertFails_PropagatesFailureAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stickerEntity = StickerEntityMock.Valid();
        var model = ImportCollectionModelMock.WithSingleRow(userId, stickerEntity.Code);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByCodeAsync(Result<StickerEntity?>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(null))
            .SetupBulkUpsertAsync(Result<int>.Failure(ResultCode.InternalError, "Erro ao persistir dados.", 500))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ImportCollectionAsync_GetByCodeFails_AddsErrorForThatLineAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = ImportCollectionModelMock.WithSingleRow(userId, "BRA01");

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByCodeAsync(Result<StickerEntity?>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Failed.ShouldBe(1);
        result.Value.Errors[0].Reason.ShouldContain("Erro ao buscar figurinha");
    }

    [Fact]
    public async Task ImportCollectionAsync_AllRowsInvalid_DoesNotCallBulkUpsertAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new ImportCollectionModel
        {
            UserId = userId,
            Rows = [new CsvStickerRowModel { StickerNumber = "", Quantity = 1 }]
        };

        var stickerRepository = new StickerRepositoryMock().Build();
        var userCollectionRepositoryMock = new UserCollectionRepositoryMock();

        var service = new CollectionService(stickerRepository, userCollectionRepositoryMock.Build(), _mapper);

        // Act
        await service.ImportCollectionAsync(model, CancellationToken.None);

        // Assert
        userCollectionRepositoryMock.VerifyBulkUpsertAsyncCalled(Times.Never());
    }
}
