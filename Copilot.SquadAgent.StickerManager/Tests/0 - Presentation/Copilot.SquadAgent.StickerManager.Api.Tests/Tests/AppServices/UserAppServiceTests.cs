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

public class UserAppServiceTests
{
    private readonly IMapper _mapper;

    public UserAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsCreatedAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.Valid();
        var userModel = UserModelMock.Valid();

        var userService = new UserServiceMock()
            .SetupRegisterAsync(Result<Domain.Models.User.UserModel>.Success(userModel))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Created<DTOs.Responses.RegisterUserResponse>>();
        var created = (Created<DTOs.Responses.RegisterUserResponse>)result;
        created.Value.ShouldNotBeNull();
        created.Value!.UserId.ShouldBe(userModel.Id);
        created.Value.Email.ShouldBe(userModel.Email);
    }

    [Fact]
    public async Task RegisterAsync_ServiceReturnsConflict_ReturnsProblemAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupRegisterAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.Conflict, "E-mail já cadastrado.", 409))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(409);
    }

    [Fact]
    public async Task RegisterAsync_ServiceReturnsInternalError_ReturnsProblemAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupRegisterAsync(Result<Domain.Models.User.UserModel>.Failure(ResultCode.InternalError, "Erro interno.", 500))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }
}
