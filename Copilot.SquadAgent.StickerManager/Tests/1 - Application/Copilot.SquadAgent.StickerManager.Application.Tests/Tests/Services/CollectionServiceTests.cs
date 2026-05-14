using AutoMapper;
using Copilot.SquadAgent.StickerManager.Application.Services.Collection;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using Shouldly;
using Xunit;
using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Services;

public class CollectionServiceTests
{
    private readonly IMapper _mapper;

    public CollectionServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task AddStickerAsync_NewSticker_ReturnsSuccessAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();
        var createdEntry = UserCollectionEntityMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(null))
            .SetupCreateAsync(Result<UserCollectionEntity>.Success(createdEntry))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.UserId.ShouldBe(createdEntry.UserId);
        result.Value.StickerId.ShouldBe(createdEntry.StickerId);
    }

    [Fact]
    public async Task AddStickerAsync_StickerNotFound_ReturnsNotFoundAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Failure(ResultCode.NotFound, "Figurinha não encontrada.", 404))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock().Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task AddStickerAsync_ExistingStickerBelowLimit_IncrementsQuantitiesAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();
        var existingEntry = UserCollectionEntityMock.WithDuplicates(5);
        var updatedEntry = UserCollectionEntityMock.WithDuplicates(6);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(existingEntry))
            .SetupUpdateAsync(Result<UserCollectionEntity>.Success(updatedEntry))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.QuantityDuplicate.ShouldBe(6);
    }

    [Fact]
    public async Task AddStickerAsync_ExistingStickerAtMaxDuplicates_ReturnsBusinessErrorAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();
        var existingEntry = UserCollectionEntityMock.WithMaxDuplicates();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(existingEntry))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.BusinessError);
        result.StatusCode.ShouldBe(422);
        result.Message!.ShouldContain("10");
    }

    [Fact]
    public async Task AddStickerAsync_GetByUserAndStickerFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task AddStickerAsync_CreateFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(null))
            .SetupCreateAsync(Result<UserCollectionEntity>.Failure(ResultCode.InternalError, "Erro ao criar coleção.", 500))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task AddStickerAsync_UpdateFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();
        var existingEntry = UserCollectionEntityMock.WithDuplicates(3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepository = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(existingEntry))
            .SetupUpdateAsync(Result<UserCollectionEntity>.Failure(ResultCode.InternalError, "Erro ao atualizar coleção.", 500))
            .Build();

        var service = new CollectionService(stickerRepository, userCollectionRepository, _mapper);

        // Act
        var result = await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task AddStickerAsync_NewSticker_DoesNotCallUpdateAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();
        var createdEntry = UserCollectionEntityMock.Valid();

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepositoryMock = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(null))
            .SetupCreateAsync(Result<UserCollectionEntity>.Success(createdEntry));

        var service = new CollectionService(stickerRepository, userCollectionRepositoryMock.Build(), _mapper);

        // Act
        await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        userCollectionRepositoryMock.VerifyUpdateAsyncCalled(Times.Never());
    }

    [Fact]
    public async Task AddStickerAsync_ExistingSticker_DoesNotCallCreateAsync()
    {
        // Arrange
        var model = AddToCollectionModelMock.Valid();
        var stickerEntity = StickerEntityMock.Valid();
        var existingEntry = UserCollectionEntityMock.WithDuplicates(2);
        var updatedEntry = UserCollectionEntityMock.WithDuplicates(3);

        var stickerRepository = new StickerRepositoryMock()
            .SetupGetByIdAsync(Result<StickerEntity>.Success(stickerEntity))
            .Build();

        var userCollectionRepositoryMock = new UserCollectionRepositoryMock()
            .SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?>.Success(existingEntry))
            .SetupUpdateAsync(Result<UserCollectionEntity>.Success(updatedEntry));

        var service = new CollectionService(stickerRepository, userCollectionRepositoryMock.Build(), _mapper);

        // Act
        await service.AddStickerAsync(model, CancellationToken.None);

        // Assert
        userCollectionRepositoryMock.VerifyCreateAsyncCalled(Times.Never());
    }
}
