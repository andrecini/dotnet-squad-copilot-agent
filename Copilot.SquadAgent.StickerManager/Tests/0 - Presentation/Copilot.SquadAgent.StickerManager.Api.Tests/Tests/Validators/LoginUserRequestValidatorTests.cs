using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Auth;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class LoginUserRequestValidatorTests
{
    private readonly LoginUserRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = LoginUserRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_EmptyEmail_FailsValidation()
    {
        // Arrange
        var request = LoginUserRequestMock.WithEmptyEmail();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserRequest.Email));
    }

    [Fact]
    public void Validate_InvalidEmailFormat_FailsValidation()
    {
        // Arrange
        var request = LoginUserRequestMock.WithInvalidEmail();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserRequest.Email));
    }

    [Fact]
    public void Validate_EmptyPassword_FailsValidation()
    {
        // Arrange
        var request = LoginUserRequestMock.WithEmptyPassword();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserRequest.Password));
    }
}
