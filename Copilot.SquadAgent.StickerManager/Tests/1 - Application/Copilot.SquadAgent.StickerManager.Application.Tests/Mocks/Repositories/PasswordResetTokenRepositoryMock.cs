using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using PasswordResetTokenEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.PasswordResetToken;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;

public class PasswordResetTokenRepositoryMock : BaseMock<IPasswordResetTokenRepository>
{
    public PasswordResetTokenRepositoryMock SetupCreateAsync(Result<PasswordResetTokenEntity> returnValue)
    {
        _mock.Setup(x => x.CreateAsync(It.IsAny<PasswordResetTokenEntity>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public PasswordResetTokenRepositoryMock SetupGetValidByTokenAsync(Result<PasswordResetTokenEntity> returnValue)
    {
        _mock.Setup(x => x.GetValidByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public PasswordResetTokenRepositoryMock SetupMarkAsUsedAsync(Result<PasswordResetTokenEntity> returnValue)
    {
        _mock.Setup(x => x.MarkAsUsedAsync(It.IsAny<PasswordResetTokenEntity>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public PasswordResetTokenRepositoryMock VerifyCreateAsyncCalled(Times times)
    {
        _mock.Verify(x => x.CreateAsync(It.IsAny<PasswordResetTokenEntity>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public PasswordResetTokenRepositoryMock VerifyMarkAsUsedAsyncCalled(Times times)
    {
        _mock.Verify(x => x.MarkAsUsedAsync(It.IsAny<PasswordResetTokenEntity>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
