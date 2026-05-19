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

public class LoginUserAppServiceTests
{
    private readonly IMapper _mapper;

    public LoginUserAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task LoginAsync_ValidRequest_ReturnsOkWithTokenAsync()
    {
        // Arrange
        var request = LoginUserRequestMock.Valid();
        var tokenModel = TokenModelMock.Valid();

        var userService = new UserServiceMock()
            .SetupLoginAsync(Result<Domain.Models.User.TokenModel>.Success(tokenModel))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok<LoginUserResponse>>();
        var ok = (Ok<LoginUserResponse>)result;
        ok.Value.ShouldNotBeNull();
        ok.Value!.AccessToken.ShouldBe(tokenModel.AccessToken);
        ok.Value.TokenType.ShouldBe("Bearer");
        ok.Value.ExpiresIn.ShouldBe(3600);
    }

    [Fact]
    public async Task LoginAsync_ServiceReturnsUnauthorized_ReturnsProblemWith401Async()
    {
        // Arrange
        var request = LoginUserRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupLoginAsync(Result<Domain.Models.User.TokenModel>.Failure(ResultCode.Unauthorized, "Credenciais inválidas.", 401))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(401);
    }

    [Fact]
    public async Task LoginAsync_ServiceReturnsInternalError_ReturnsProblemWith500Async()
    {
        // Arrange
        var request = LoginUserRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupLoginAsync(Result<Domain.Models.User.TokenModel>.Failure(ResultCode.InternalError, "Erro interno.", 500))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task LoginAsync_ServiceReturnsFailureWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var request = LoginUserRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupLoginAsync(Result<Domain.Models.User.TokenModel>.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }
}
