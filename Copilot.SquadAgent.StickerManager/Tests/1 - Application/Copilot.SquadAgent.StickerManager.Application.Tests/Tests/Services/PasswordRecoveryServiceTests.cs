using AutoMapper;
using Copilot.SquadAgent.StickerManager.Application.Services.User;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;
using Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;
using Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Security;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;
using PasswordResetTokenEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.PasswordResetToken;
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Services;

public class PasswordRecoveryServiceTests
{
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger = new Mock<ILogger<UserService>>().Object;

    public PasswordRecoveryServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.UserProfile>();
        }).CreateMapper();
    }

    private UserService BuildService(
        UserRepositoryMock? userRepoMock = null,
        PasswordResetTokenRepositoryMock? tokenRepoMock = null,
        PasswordHasherMock? hasherMock = null)
    {
        return new UserService(
            (userRepoMock ?? new UserRepositoryMock()).Build(),
            (tokenRepoMock ?? new PasswordResetTokenRepositoryMock()).Build(),
            (hasherMock ?? new PasswordHasherMock()).Build(),
            new JwtTokenGeneratorMock().Build(),
            _logger,
            _mapper);
    }

    // -------- ForgotPasswordAsync --------

    [Fact]
    public async Task ForgotPasswordAsync_ExistingEmail_ReturnsSuccessAsync()
    {
        // Arrange
        var model = ForgotPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupCreateAsync(Result<PasswordResetTokenEntity>.Success(token));

        var service = BuildService(userRepoMock, tokenRepoMock);

        // Act
        var result = await service.ForgotPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ForgotPasswordAsync_EmailNotFound_ReturnsSuccessWithoutRevealingAsync()
    {
        // Arrange
        var model = ForgotPasswordModelMock.Valid();

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404));

        var service = BuildService(userRepoMock);

        // Act
        var result = await service.ForgotPasswordAsync(model, CancellationToken.None);

        // Assert — não expõe que o e-mail não existe
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ForgotPasswordAsync_EmailNotFound_DoesNotCreateTokenAsync()
    {
        // Arrange
        var model = ForgotPasswordModelMock.Valid();

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock();

        var service = BuildService(userRepoMock, tokenRepoMock);

        // Act
        await service.ForgotPasswordAsync(model, CancellationToken.None);

        // Assert
        tokenRepoMock.VerifyCreateAsyncCalled(Times.Never());
    }

    [Fact]
    public async Task ForgotPasswordAsync_ExistingEmail_CreatesTokenAsync()
    {
        // Arrange
        var model = ForgotPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupCreateAsync(Result<PasswordResetTokenEntity>.Success(token));

        var service = BuildService(userRepoMock, tokenRepoMock);

        // Act
        await service.ForgotPasswordAsync(model, CancellationToken.None);

        // Assert
        tokenRepoMock.VerifyCreateAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task ForgotPasswordAsync_RepositoryCreateFails_ReturnsFailureAsync()
    {
        // Arrange
        var model = ForgotPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupCreateAsync(Result<PasswordResetTokenEntity>.Failure(ResultCode.InternalError, "Erro de banco.", 500));

        var service = BuildService(userRepoMock, tokenRepoMock);

        // Act
        var result = await service.ForgotPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    // -------- ResetPasswordAsync --------

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_ReturnsSuccessAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();
        token.UserId = userEntity.Id;

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Success(token))
            .SetupMarkAsUsedAsync(Result<PasswordResetTokenEntity>.Success(token));

        var hasherMock = new PasswordHasherMock().SetupHash();

        var service = BuildService(userRepoMock, tokenRepoMock, hasherMock);

        // Act
        var result = await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ResetPasswordAsync_InvalidToken_ReturnsValidationErrorAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Failure(ResultCode.NotFound, "Token não encontrado.", 404));

        var service = BuildService(tokenRepoMock: tokenRepoMock);

        // Act
        var result = await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.ValidationError);
        result.StatusCode.ShouldBe(400);
        result.Message.ShouldBe("Token inválido ou expirado.");
    }

    [Fact]
    public async Task ResetPasswordAsync_UserNotFound_ReturnsNotFoundAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Success(token));

        var service = BuildService(userRepoMock, tokenRepoMock);

        // Act
        var result = await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.NotFound);
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_HashesNewPasswordAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();
        token.UserId = userEntity.Id;

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Success(token))
            .SetupMarkAsUsedAsync(Result<PasswordResetTokenEntity>.Success(token));

        var hasherMock = new PasswordHasherMock().SetupHash();

        var service = BuildService(userRepoMock, tokenRepoMock, hasherMock);

        // Act
        await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        hasherMock.VerifyHashCalled(Times.Once());
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_MarksTokenAsUsedAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();
        token.UserId = userEntity.Id;

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Success(token))
            .SetupMarkAsUsedAsync(Result<PasswordResetTokenEntity>.Success(token));

        var hasherMock = new PasswordHasherMock().SetupHash();

        var service = BuildService(userRepoMock, tokenRepoMock, hasherMock);

        // Act
        await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        tokenRepoMock.VerifyMarkAsUsedAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task ResetPasswordAsync_UpdateUserFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();
        token.UserId = userEntity.Id;

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Failure(ResultCode.InternalError, "Erro ao atualizar.", 500));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Success(token));

        var hasherMock = new PasswordHasherMock().SetupHash();

        var service = BuildService(userRepoMock, tokenRepoMock, hasherMock);

        // Act
        var result = await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ResetPasswordAsync_MarkAsUsedFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var token = PasswordResetTokenEntityMock.Valid();
        token.UserId = userEntity.Id;

        var userRepoMock = new UserRepositoryMock()
            .SetupGetByIdAsync(Result<UserEntity>.Success(userEntity))
            .SetupUpdateAsync(Result<UserEntity>.Success(userEntity));

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Success(token))
            .SetupMarkAsUsedAsync(Result<PasswordResetTokenEntity>.Failure(ResultCode.InternalError, "Erro ao invalidar token.", 500));

        var hasherMock = new PasswordHasherMock().SetupHash();

        var service = BuildService(userRepoMock, tokenRepoMock, hasherMock);

        // Act
        var result = await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ResetPasswordAsync_InvalidToken_DoesNotCallUserRepositoryAsync()
    {
        // Arrange
        var model = ResetPasswordModelMock.Valid();

        var userRepoMock = new UserRepositoryMock();

        var tokenRepoMock = new PasswordResetTokenRepositoryMock()
            .SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity>.Failure(ResultCode.NotFound, "Token não encontrado.", 404));

        var service = BuildService(userRepoMock, tokenRepoMock);

        // Act
        await service.ResetPasswordAsync(model, CancellationToken.None);

        // Assert
        userRepoMock.VerifyUpdateAsyncCalled(Times.Never());
    }
}
