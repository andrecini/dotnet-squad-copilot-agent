using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;

public class UserCollectionRepositoryMock : BaseMock<IUserCollectionRepository>
{
    public UserCollectionRepositoryMock SetupGetByUserAndStickerAsync(Result<UserCollectionEntity?> returnValue)
    {
        _mock.Setup(x => x.GetByUserAndStickerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserCollectionRepositoryMock SetupCreateAsync(Result<UserCollectionEntity> returnValue)
    {
        _mock.Setup(x => x.CreateAsync(It.IsAny<UserCollectionEntity>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserCollectionRepositoryMock SetupUpdateAsync(Result<UserCollectionEntity> returnValue)
    {
        _mock.Setup(x => x.UpdateAsync(It.IsAny<UserCollectionEntity>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public UserCollectionRepositoryMock VerifyCreateAsyncCalled(Times times)
    {
        _mock.Verify(x => x.CreateAsync(It.IsAny<UserCollectionEntity>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public UserCollectionRepositoryMock VerifyUpdateAsyncCalled(Times times)
    {
        _mock.Verify(x => x.UpdateAsync(It.IsAny<UserCollectionEntity>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
