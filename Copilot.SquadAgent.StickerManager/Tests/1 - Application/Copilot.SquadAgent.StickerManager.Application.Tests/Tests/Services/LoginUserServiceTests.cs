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
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Services;

public class LoginUserServiceTests
{
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger = new Mock<ILogger<UserService>>().Object;

    public LoginUserServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.UserProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithTokenAsync()
    {
        // Arrange
        var model = LoginUserModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var tokenModel = TokenModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var passwordHasher = new PasswordHasherMock()
            .SetupVerify(true)
            .Build();

        var jwtGenerator = new JwtTokenGeneratorMock()
            .SetupGenerate(tokenModel)
            .Build();

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasher, jwtGenerator, _logger, _mapper);

        // Act
        var result = await service.LoginAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.AccessToken.ShouldBe(tokenModel.AccessToken);
        result.Value.TokenType.ShouldBe("Bearer");
        result.Value.ExpiresIn.ShouldBe(3600);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsUnauthorizedAsync()
    {
        // Arrange
        var model = LoginUserModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404))
            .Build();

        var passwordHasher = new PasswordHasherMock().Build();
        var jwtGenerator = new JwtTokenGeneratorMock().Build();

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasher, jwtGenerator, _logger, _mapper);

        // Act
        var result = await service.LoginAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.Unauthorized);
        result.StatusCode.ShouldBe(401);
        result.Message.ShouldBe("Credenciais inválidas.");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsUnauthorizedAsync()
    {
        // Arrange
        var model = LoginUserModelMock.WithWrongPassword();
        var userEntity = UserEntityMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var passwordHasher = new PasswordHasherMock()
            .SetupVerify(false)
            .Build();

        var jwtGenerator = new JwtTokenGeneratorMock().Build();

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasher, jwtGenerator, _logger, _mapper);

        // Act
        var result = await service.LoginAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.Unauthorized);
        result.StatusCode.ShouldBe(401);
        result.Message.ShouldBe("Credenciais inválidas.");
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_DoesNotCallPasswordVerifyAsync()
    {
        // Arrange
        var model = LoginUserModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404))
            .Build();

        var passwordHasherMock = new PasswordHasherMock();
        var jwtGenerator = new JwtTokenGeneratorMock().Build();

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasherMock.Build(), jwtGenerator, _logger, _mapper);

        // Act
        await service.LoginAsync(model, CancellationToken.None);

        // Assert
        passwordHasherMock.VerifyVerifyCalled(Times.Never());
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_DoesNotCallJwtGeneratorAsync()
    {
        // Arrange
        var model = LoginUserModelMock.WithWrongPassword();
        var userEntity = UserEntityMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var passwordHasher = new PasswordHasherMock()
            .SetupVerify(false)
            .Build();

        var jwtGeneratorMock = new JwtTokenGeneratorMock();

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasher, jwtGeneratorMock.Build(), _logger, _mapper);

        // Act
        await service.LoginAsync(model, CancellationToken.None);

        // Assert
        jwtGeneratorMock.VerifyGenerateCalled(Times.Never());
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_CallsPasswordVerifyAsync()
    {
        // Arrange
        var model = LoginUserModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var tokenModel = TokenModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var passwordHasherMock = new PasswordHasherMock()
            .SetupVerify(true);

        var jwtGenerator = new JwtTokenGeneratorMock()
            .SetupGenerate(tokenModel)
            .Build();

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasherMock.Build(), jwtGenerator, _logger, _mapper);

        // Act
        await service.LoginAsync(model, CancellationToken.None);

        // Assert
        passwordHasherMock.VerifyVerifyCalled(Times.Once());
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_CallsJwtGeneratorAsync()
    {
        // Arrange
        var model = LoginUserModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var tokenModel = TokenModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupGetByEmailAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var passwordHasher = new PasswordHasherMock()
            .SetupVerify(true)
            .Build();

        var jwtGeneratorMock = new JwtTokenGeneratorMock()
            .SetupGenerate(tokenModel);

        var service = new UserService(userRepository, new PasswordResetTokenRepositoryMock().Build(), passwordHasher, jwtGeneratorMock.Build(), _logger, _mapper);

        // Act
        await service.LoginAsync(model, CancellationToken.None);

        // Assert
        jwtGeneratorMock.VerifyGenerateCalled(Times.Once());
    }
}
