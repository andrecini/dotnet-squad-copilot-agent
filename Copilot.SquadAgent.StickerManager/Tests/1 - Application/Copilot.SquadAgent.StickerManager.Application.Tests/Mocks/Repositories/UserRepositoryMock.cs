using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;

public class UserRepositoryMock : BaseMock<IUserRepository>
{
    public UserRepositoryMock SetupExistsByEmailAsync(bool returnValue)
    {
        _mock.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserRepositoryMock SetupCreateAsync(Result<UserEntity> returnValue)
    {
        _mock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserRepositoryMock VerifyCreateAsyncNotCalled()
    {
        _mock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        return this;
    }
}
