using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class PasswordRecoveryAppServiceTests
{
    private readonly IMapper _mapper;

    public PasswordRecoveryAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        }).CreateMapper();
    }

    // -------- ForgotPasswordAsync --------

    [Fact]
    public async Task ForgotPasswordAsync_ValidRequest_ReturnsOkAsync()
    {
        // Arrange
        var request = ForgotPasswordRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupForgotPasswordAsync(Result.Success())
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.ForgotPasswordAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok>();
    }

    [Fact]
    public async Task ForgotPasswordAsync_ServiceReturnsFailure_ReturnsProblemAsync()
    {
        // Arrange
        var request = ForgotPasswordRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupForgotPasswordAsync(Result.Failure(ResultCode.InternalError, "Erro interno.", 500))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.ForgotPasswordAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ServiceReturnsFailureWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var request = ForgotPasswordRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupForgotPasswordAsync(Result.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.ForgotPasswordAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }

    // -------- ResetPasswordAsync --------

    [Fact]
    public async Task ResetPasswordAsync_ValidRequest_ReturnsOkAsync()
    {
        // Arrange
        var request = ResetPasswordRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupResetPasswordAsync(Result.Success())
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<Ok>();
    }

    [Fact]
    public async Task ResetPasswordAsync_ServiceReturnsValidationError_ReturnsProblemAsync()
    {
        // Arrange
        var request = ResetPasswordRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupResetPasswordAsync(Result.Failure(ResultCode.ValidationError, "Token inválido ou expirado.", 400))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task ResetPasswordAsync_ServiceReturnsFailureWithNullStatusCode_ReturnsProblemWith500Async()
    {
        // Arrange
        var request = ResetPasswordRequestMock.Valid();

        var userService = new UserServiceMock()
            .SetupResetPasswordAsync(Result.Failure(ResultCode.InternalError, "Erro interno."))
            .Build();

        var appService = new UserAppService(userService, _mapper);

        // Act
        var result = await appService.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result;
        problem.StatusCode.ShouldBe(500);
    }
}
