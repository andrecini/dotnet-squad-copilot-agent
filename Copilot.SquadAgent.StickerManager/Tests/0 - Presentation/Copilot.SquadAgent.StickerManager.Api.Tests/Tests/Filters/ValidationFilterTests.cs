using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Filters;

public class ValidationFilterTests
{
    [Fact]
    public async Task InvokeAsync_ValidRequest_CallsNextAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.Valid();

        var validatorMock = new ValidatorMock<RegisterUserRequest>()
            .SetupValidAsync();

        var context = new Mock<EndpointFilterInvocationContext>();
        context.Setup(c => c.Arguments).Returns(new List<object?> { request });

        var nextCalled = false;
        EndpointFilterDelegate next = _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(TypedResults.Ok());
        };

        var filter = new ValidationFilter<RegisterUserRequest>(validatorMock.Build());

        // Act
        await filter.InvokeAsync(context.Object, next);

        // Assert
        nextCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task InvokeAsync_NullRequest_ReturnsBadRequestAsync()
    {
        // Arrange
        var validatorMock = new ValidatorMock<RegisterUserRequest>();

        var context = new Mock<EndpointFilterInvocationContext>();
        context.Setup(c => c.Arguments).Returns(new List<object?> { "outro-tipo-qualquer" });

        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>(TypedResults.Ok());

        var filter = new ValidationFilter<RegisterUserRequest>(validatorMock.Build());

        // Act
        var result = await filter.InvokeAsync(context.Object, next);

        // Assert
        result.ShouldBeOfType<BadRequest>();
    }

    [Fact]
    public async Task InvokeAsync_InvalidRequest_ReturnsValidationProblemAsync()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithEmptyEmail();

        var failures = new[] { new ValidationFailure(nameof(RegisterUserRequest.Email), "E-mail inválido.") };
        var validatorMock = new ValidatorMock<RegisterUserRequest>()
            .SetupInvalidAsync(failures);

        var context = new Mock<EndpointFilterInvocationContext>();
        context.Setup(c => c.Arguments).Returns(new List<object?> { request });

        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>(TypedResults.Ok());

        var filter = new ValidationFilter<RegisterUserRequest>(validatorMock.Build());

        // Act
        var result = await filter.InvokeAsync(context.Object, next);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problem = (ProblemHttpResult)result!;
        problem.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }
}
