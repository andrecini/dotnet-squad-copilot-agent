using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Collection;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class AddToCollectionRequestValidatorTests
{
    private readonly AddToCollectionRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = AddToCollectionRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_EmptyStickerId_FailsValidation()
    {
        // Arrange
        var request = AddToCollectionRequestMock.WithEmptyStickerId();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(AddToCollectionRequest.StickerId));
    }

    [Fact]
    public void Validate_EmptyStickerId_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = AddToCollectionRequestMock.WithEmptyStickerId();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O identificador da figurinha é obrigatório.");
    }
}
