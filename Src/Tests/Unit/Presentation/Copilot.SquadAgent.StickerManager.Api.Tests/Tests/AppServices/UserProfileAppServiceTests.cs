using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class UserProfileAppServiceTests
{
    private readonly IMapper _mapper;

    public UserProfileAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task GetProfileAsync_UserFound_ReturnsOkWithProfileAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userModel = UserModelMock.Valid();

        var userService = new UserServiceMock()
            .SetupGetProfileAsync(Result<Domain.Models.User.UserModel>.Success(userModel))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.GetProfileAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<UserProfileResponse>>();
        var ok = (Ok<UserProfileResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Email.ShouldBe(userModel.Email);
        ok.Value.Name.ShouldBe(userModel.Name);
        ok.Value.UserId.ShouldBe(userModel.Id);
    }

    [Fact]
    public async Task GetProfileAsync_UserNotFound_ReturnsProblemAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userService = new UserServiceMock()
            .SetupGetProfileAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.GetProfileAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetProfileAsync_ServiceReturnsInternalError_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userService = new UserServiceMock()
            .SetupGetProfileAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.GetProfileAsync(userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task UpdateProfileAsync_ValidRequest_ReturnsOkWithUpdatedProfileAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = UpdateUserProfileRequestMock.Valid();
        var userModel = UserModelMock.Valid();

        var userService = new UserServiceMock()
            .SetupUpdateProfileAsync(Result<Domain.Models.User.UserModel>.Success(userModel))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.UpdateProfileAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<UserProfileResponse>>();
        var ok = (Ok<UserProfileResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.Email.ShouldBe(userModel.Email);
        ok.Value.Name.ShouldBe(userModel.Name);
    }

    [Fact]
    public async Task UpdateProfileAsync_EmailConflict_ReturnsProblemWith409Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = UpdateUserProfileRequestMock.WithOnlyEmail();

        var userService = new UserServiceMock()
            .SetupUpdateProfileAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.Conflict, "E-mail já cadastrado.", 409))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.UpdateProfileAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(409);
    }

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_ReturnsProblemWith404Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = UpdateUserProfileRequestMock.WithOnlyName();

        var userService = new UserServiceMock()
            .SetupUpdateProfileAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.NotFound, "Usuário não encontrado.", 404))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.UpdateProfileAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task UpdateProfileAsync_ServiceReturnsFailureWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = UpdateUserProfileRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupUpdateProfileAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.UpdateProfileAsync(userId, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }
}
