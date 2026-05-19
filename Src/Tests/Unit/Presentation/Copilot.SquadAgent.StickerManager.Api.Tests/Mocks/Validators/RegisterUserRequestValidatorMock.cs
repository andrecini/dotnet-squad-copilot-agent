using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Validators;

public class RegisterUserRequestValidatorMock : BaseMock<IValidator<RegisterUserRequest>>
{
    public RegisterUserRequestValidatorMock SetupValidateAsync(ValidationResult returnValue)
    {
        _mock.Setup(x => x.ValidateAsync(It.IsAny<RegisterUserRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }
}
