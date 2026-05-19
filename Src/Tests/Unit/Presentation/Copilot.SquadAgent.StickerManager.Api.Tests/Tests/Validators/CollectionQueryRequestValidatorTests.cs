using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Collection;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class CollectionQueryRequestValidatorTests
{
    private readonly CollectionQueryRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = CollectionQueryRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_PageZero_FailsValidation()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithPageZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CollectionQueryRequest.Page));
    }

    [Fact]
    public void Validate_PageZero_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithPageZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "A página deve ser maior ou igual a 1.");
    }

    [Fact]
    public void Validate_PageSizeZero_FailsValidation()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithPageSizeZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CollectionQueryRequest.PageSize));
    }

    [Fact]
    public void Validate_PageSizeZero_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithPageSizeZero();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O limite deve estar entre 1 e 500.");
    }

    [Fact]
    public void Validate_PageSizeAboveMaximum_FailsValidation()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithPageSizeAboveMaximum();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CollectionQueryRequest.PageSize));
    }

    [Fact]
    public void Validate_PageSizeAboveMaximum_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithPageSizeAboveMaximum();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O limite deve estar entre 1 e 500.");
    }

    [Fact]
    public void Validate_InvalidSortValue_FailsValidation()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithInvalidSort();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CollectionQueryRequest.Sort));
    }

    [Fact]
    public void Validate_InvalidSortValue_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithInvalidSort();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O campo sort deve ser um dos valores: number, player_name, acquired_at.");
    }

    [Fact]
    public void Validate_NullSort_PassesValidation()
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithNullSort();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("number")]
    [InlineData("player_name")]
    [InlineData("acquired_at")]
    public void Validate_AllAllowedSortValues_PassValidation(string sortValue)
    {
        // Arrange
        var request = CollectionQueryRequestMock.WithSort(sortValue);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}
