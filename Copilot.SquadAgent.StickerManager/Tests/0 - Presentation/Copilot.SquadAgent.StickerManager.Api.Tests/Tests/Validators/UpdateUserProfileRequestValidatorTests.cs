using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.User;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class UpdateUserProfileRequestValidatorTests
{
    private readonly UpdateUserProfileRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_PassesValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.Valid();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_OnlyNameProvided_PassesValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.WithOnlyName();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_OnlyEmailProvided_PassesValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.WithOnlyEmail();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_InvalidEmail_FailsValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.WithInvalidEmail();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task ValidateAsync_NameTooShort_FailsValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.WithShortName();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task ValidateAsync_NameTooLong_FailsValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.WithOnlyName();
        request.Name = new string('A', 257);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task ValidateAsync_BothFieldsNull_FailsValidationAsync()
    {
        // Arrange
        var request = UpdateUserProfileRequestMock.WithBothNull();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "request");
    }
}
