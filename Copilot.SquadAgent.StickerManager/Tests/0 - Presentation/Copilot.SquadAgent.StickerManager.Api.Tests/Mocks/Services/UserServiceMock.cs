using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;

public class UserServiceMock : BaseMock<IUserService>
{
    public UserServiceMock SetupRegisterAsync(Result<UserModel> returnValue)
    {
        _mock.Setup(x => x.RegisterAsync(It.IsAny<RegisterUserModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserServiceMock VerifyRegisterAsyncCalled(Times times)
    {
        _mock.Verify(x => x.RegisterAsync(It.IsAny<RegisterUserModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public UserServiceMock SetupLoginAsync(Result<TokenModel> returnValue)
    {
        _mock.Setup(x => x.LoginAsync(It.IsAny<LoginUserModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserServiceMock VerifyLoginAsyncCalled(Times times)
    {
        _mock.Verify(x => x.LoginAsync(It.IsAny<LoginUserModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public UserServiceMock SetupGetProfileAsync(Result<UserModel> returnValue)
    {
        _mock.Setup(x => x.GetProfileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserServiceMock SetupUpdateProfileAsync(Result<UserModel> returnValue)
    {
        _mock.Setup(x => x.UpdateProfileAsync(It.IsAny<UpdateUserProfileModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserServiceMock SetupForgotPasswordAsync(Result returnValue)
    {
        _mock.Setup(x => x.ForgotPasswordAsync(It.IsAny<ForgotPasswordModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserServiceMock SetupResetPasswordAsync(Result returnValue)
    {
        _mock.Setup(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }
}
