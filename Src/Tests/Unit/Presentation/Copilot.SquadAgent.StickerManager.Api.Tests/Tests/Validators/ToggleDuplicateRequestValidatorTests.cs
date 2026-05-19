using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class ToggleDuplicateRequestValidatorTests
{
    private readonly ToggleDuplicateRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidMarkAction_PassesValidation()
    {
        // Arrange
        var request = ToggleDuplicateRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ValidUnmarkAction_PassesValidation()
    {
        // Arrange
        var request = ToggleDuplicateRequestMock.WithUnmarkAction();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_InvalidEnumValue_FailsValidation()
    {
        // Arrange
        var request = new ToggleDuplicateRequest { Action = (DuplicateAction)99 };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(ToggleDuplicateRequest.Action));
    }
}
