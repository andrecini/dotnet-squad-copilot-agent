using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Auth;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_PassesValidationAsync()
    {
        var request = ResetPasswordRequestMock.Valid();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateAsync_EmptyToken_FailsValidationAsync()
    {
        var request = ResetPasswordRequestMock.WithEmptyToken();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Token");
    }

    [Fact]
    public async Task ValidateAsync_WeakPassword_FailsValidationAsync()
    {
        var request = ResetPasswordRequestMock.WithWeakPassword();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewPassword");
    }

    [Fact]
    public async Task ValidateAsync_PasswordWithNoUpperCase_FailsValidationAsync()
    {
        var request = ResetPasswordRequestMock.WithNoUpperCase();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewPassword");
    }

    [Fact]
    public async Task ValidateAsync_PasswordWithNoNumber_FailsValidationAsync()
    {
        var request = ResetPasswordRequestMock.WithNoNumber();
        var result = await _validator.ValidateAsync(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewPassword");
    }
}
