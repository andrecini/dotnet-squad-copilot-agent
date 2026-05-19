using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Collection;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class MissingStickersQueryRequestValidatorTests
{
    private readonly MissingStickersQueryRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_PageZero_FailsValidation()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithPageZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(MissingStickersQueryRequest.Page));
    }

    [Fact]
    public void Validate_PageZero_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithPageZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "A página deve ser maior ou igual a 1.");
    }

    [Fact]
    public void Validate_PageSizeZero_FailsValidation()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithPageSizeZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(MissingStickersQueryRequest.PageSize));
    }

    [Fact]
    public void Validate_PageSizeZero_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithPageSizeZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O limite deve estar entre 1 e 500.");
    }

    [Fact]
    public void Validate_PageSizeAboveMaximum_FailsValidation()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithPageSizeAboveMaximum();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(MissingStickersQueryRequest.PageSize));
    }

    [Fact]
    public void Validate_PageSizeAboveMaximum_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithPageSizeAboveMaximum();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O limite deve estar entre 1 e 500.");
    }

    [Fact]
    public void Validate_InvalidSortValue_FailsValidation()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithInvalidSort();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(MissingStickersQueryRequest.Sort));
    }

    [Fact]
    public void Validate_InvalidSortValue_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithInvalidSort();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O campo sort deve ser um dos valores: number, team, rarity.");
    }

    [Fact]
    public void Validate_NullSort_PassesValidation()
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithNullSort();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("number")]
    [InlineData("team")]
    [InlineData("rarity")]
    public void Validate_AllAllowedSortValues_PassValidation(string sortValue)
    {
        // Arrange
        var request = MissingStickersQueryRequestMock.WithSort(sortValue);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}
