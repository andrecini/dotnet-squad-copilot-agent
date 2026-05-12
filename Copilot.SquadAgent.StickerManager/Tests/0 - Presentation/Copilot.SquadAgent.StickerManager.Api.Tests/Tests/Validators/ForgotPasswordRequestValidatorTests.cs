using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Auth;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_PassesValidationAsync()
    {
        var request = ForgotPasswordRequestMock.Valid();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_EmptyEmail_FailsValidationAsync()
    {
        var request = ForgotPasswordRequestMock.WithEmptyEmail();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task ValidateAsync_InvalidEmailFormat_FailsValidationAsync()
    {
        var request = ForgotPasswordRequestMock.WithInvalidEmail();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Email");
    }
}
