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

public class UserServiceTests
{
    private readonly IMapper _mapper;

    public UserServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Domain.Mappings.UserProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task RegisterAsync_ValidModel_ReturnsSuccessAsync()
    {
        // Arrange
        var model = RegisterUserModelMock.Valid();
        var userEntity = UserEntityMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupExistsByEmailAsync(false)
            .SetupCreateAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var passwordHasher = new PasswordHasherMock()
            .SetupHash()
            .Build();

        var jwtGenerator = new JwtTokenGeneratorMock().Build();
        var service = new UserService(userRepository, passwordHasher, jwtGenerator, _mapper);

        // Act
        var result = await service.RegisterAsync(model, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Email.ShouldBe(userEntity.Email);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ReturnsConflictAsync()
    {
        // Arrange
        var model = RegisterUserModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupExistsByEmailAsync(true)
            .Build();

        var passwordHasher = new PasswordHasherMock().Build();
        var jwtGenerator = new JwtTokenGeneratorMock().Build();

        var service = new UserService(userRepository, passwordHasher, jwtGenerator, _mapper);

        // Act
        var result = await service.RegisterAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.Conflict);
        result.StatusCode.ShouldBe(409);
    }

    [Fact]
    public async Task RegisterAsync_RepositoryFails_PropagatesFailureAsync()
    {
        // Arrange
        var model = RegisterUserModelMock.Valid();

        var userRepository = new UserRepositoryMock()
            .SetupExistsByEmailAsync(false)
            .SetupCreateAsync(Result<UserEntity>.Failure(ResultCode.InternalError, "Erro de banco.", 500))
            .Build();

        var passwordHasher = new PasswordHasherMock()
            .SetupHash()
            .Build();

        var jwtGenerator = new JwtTokenGeneratorMock().Build();
        var service = new UserService(userRepository, passwordHasher, jwtGenerator, _mapper);

        // Act
        var result = await service.RegisterAsync(model, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Code.ShouldBe(ResultCode.InternalError);
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task RegisterAsync_ValidModel_HashesPasswordBeforeSavingAsync()
    {
        // Arrange
        var model = RegisterUserModelMock.Valid();
        var userEntity = UserEntityMock.Valid();
        var passwordHasherMock = new PasswordHasherMock()
            .SetupHash();

        var userRepository = new UserRepositoryMock()
            .SetupExistsByEmailAsync(false)
            .SetupCreateAsync(Result<UserEntity>.Success(userEntity))
            .Build();

        var jwtGenerator = new JwtTokenGeneratorMock().Build();
        var service = new UserService(userRepository, passwordHasherMock.Build(), jwtGenerator, _mapper);

        // Act
        await service.RegisterAsync(model, CancellationToken.None);

        // Assert
        passwordHasherMock.VerifyHashCalled(Times.Once());
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_DoesNotCallRepositoryCreateAsync()
    {
        // Arrange
        var model = RegisterUserModelMock.Valid();
        var userRepositoryMock = new UserRepositoryMock()
            .SetupExistsByEmailAsync(true);

        var passwordHasher = new PasswordHasherMock().Build();
        var jwtGenerator = new JwtTokenGeneratorMock().Build();

        var service = new UserService(userRepositoryMock.Build(), passwordHasher, jwtGenerator, _mapper);

        // Act
        await service.RegisterAsync(model, CancellationToken.None);

        // Assert
        userRepositoryMock.VerifyCreateAsyncNotCalled();
    }
}
