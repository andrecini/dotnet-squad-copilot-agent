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
}
