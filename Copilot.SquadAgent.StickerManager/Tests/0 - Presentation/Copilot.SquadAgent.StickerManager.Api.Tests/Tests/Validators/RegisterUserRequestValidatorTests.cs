using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Auth;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class RegisterUserRequestValidatorTests
{
    private readonly RegisterUserRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_EmptyEmail_FailsValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithEmptyEmail();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserRequest.Email));
    }

    [Fact]
    public void Validate_InvalidEmailFormat_FailsValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithInvalidEmail();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserRequest.Email));
    }

    [Fact]
    public void Validate_EmptyName_FailsValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithEmptyName();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserRequest.Name));
    }

    [Fact]
    public void Validate_PasswordTooShort_FailsValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithWeakPassword();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserRequest.Password));
    }

    [Fact]
    public void Validate_PasswordWithoutUppercase_FailsValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithPasswordWithoutUppercase();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserRequest.Password));
    }

    [Fact]
    public void Validate_PasswordWithoutNumber_FailsValidation()
    {
        // Arrange
        var request = RegisterUserRequestMock.WithPasswordWithoutNumber();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserRequest.Password));
    }
}
