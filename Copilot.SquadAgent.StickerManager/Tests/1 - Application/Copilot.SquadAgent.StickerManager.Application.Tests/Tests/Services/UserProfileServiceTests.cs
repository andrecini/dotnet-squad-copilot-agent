using AutoMapper;
using Copilot.SquadAgent.StickerManager.Application.Services.User;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Security;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using Shouldly;
using Xunit;
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Services;

public class UserProfileServiceTests
{
    private readonly IMapper _mapper;

    public UserProfileServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.UserProfile>();
        }).CreateMapper();
    }

    private UserService BuildService(UserRepositoryMock repoMock) =>
        new UserService(
            repoMock.Build(),
            new PasswordHasherMock().Build(),
            new JwtTokenGeneratorMock().Build(),
            _mapper);

    // -------- GetProfileAsync --------

    [Fact]
    public async Task GetProfileAsync_UserExists_ReturnsSuccessWithUserModelAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEntity = UserEntityMock.Valid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity));

        var service = BuildService(repoMock);

        // Act
        var result = await service.GetProfileAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Email.ShouldBe(userEntity.Email);
        result.Value.Name.ShouldBe(userEntity.Name);
    }

    [Fact]
    public async Task GetProfileAsync_UserNotFound_PropagatesNotFoundFailureAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404));

        var service = BuildService(repoMock);

        // Act
        var result = await service.GetProfileAsync(userId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
        result.Message.ShouldBe("Usuário não encontrado.");
    }

    // -------- UpdateProfileAsync --------

    [Fact]
    public async Task UpdateProfileAsync_ValidModel_ReturnsSuccessWithUpdatedUserAsync()
    {
        // Arrange
        var model = UpdateUserProfileModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var updatedEntity = UserEntityMock.Valid();
        updatedEntity.Email = model.Email!;
        updatedEntity.Name = model.Name!;

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupExistsByEmailAsync(false)
            .SetupUpdateAsync(Result<UserEntity>.Success(updatedEntity));

        var service = BuildService(repoMock);

        // Act
        var result = await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Email.ShouldBe(updatedEntity.Email);
        result.Value.Name.ShouldBe(updatedEntity.Name);
    }

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_ReturnsNotFoundFailureAsync()
    {
        // Arrange
        var model = UpdateUserProfileModelMock.Valid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404));

        var service = BuildService(repoMock);

        // Act
        var result = await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task UpdateProfileAsync_NewEmailAlreadyExists_ReturnsConflictAsync()
    {
        // Arrange
        var model = UpdateUserProfileModelMock.WithOnlyEmail();
        var userEntity = UserEntityMock.Valid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupExistsByEmailAsync(true);

        var service = BuildService(repoMock);

        // Act
        var result = await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.Conflict);
        result.StatusCode.ShouldBe(409);
        result.Message.ShouldBe("E-mail já cadastrado.");
    }

    [Fact]
    public async Task UpdateProfileAsync_SameEmail_DoesNotCheckEmailConflictAsync()
    {
        // Arrange
        var userEntity = UserEntityMock.Valid();
        var model = UpdateUserProfileModelMock.WithSameEmail(userEntity.Email);

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Success(userEntity));

        var service = BuildService(repoMock);

        // Act
        var result = await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateProfileAsync_OnlyNameProvided_DoesNotCheckEmailConflictAsync()
    {
        // Arrange
        var model = UpdateUserProfileModelMock.WithOnlyName();
        var userEntity = UserEntityMock.Valid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Success(userEntity));

        var service = BuildService(repoMock);

        // Act
        var result = await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateProfileAsync_RepositoryUpdateFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = UpdateUserProfileModelMock.WithOnlyName();
        var userEntity = UserEntityMock.Valid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Failure(ResultCode.InternalError, "Erro ao atualizar.", 500));

        var service = BuildService(repoMock);

        // Act
        var result = await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task UpdateProfileAsync_NewEmailConflict_DoesNotCallUpdateAsync()
    {
        // Arrange
        var model = UpdateUserProfileModelMock.WithOnlyEmail();
        var userEntity = UserEntityMock.Valid();

        var repoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupExistsByEmailAsync(true);

        var service = BuildService(repoMock);

        // Act
        await service.UpdateProfileAsync(model, CancellationToken.None);

        // Assert
        repoMock.VerifyUpdateAsyncCalled(Times.Never());
    }
}
