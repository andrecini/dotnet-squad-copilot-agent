using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Validators;

public class ValidatorMock<T> : BaseMock<IValidator<T>> where T : class
{
    public ValidatorMock<T> SetupValidAsync()
    {
        _mock.Setup(x => x.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new ValidationResult());

        return this;
    }

    public ValidatorMock<T> SetupInvalidAsync(params ValidationFailure[] failures)
    {
        _mock.Setup(x => x.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new ValidationResult(failures));

        return this;
    }

    public ValidatorMock<T> VerifyValidateAsyncCalled(Times times)
    {
        _mock.Verify(x => x.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
