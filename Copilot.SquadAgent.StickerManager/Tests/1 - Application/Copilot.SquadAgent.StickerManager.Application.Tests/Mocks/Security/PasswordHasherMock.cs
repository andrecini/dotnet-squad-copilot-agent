using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;
using Moq;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Security;

public class PasswordHasherMock : BaseMock<IPasswordHasher>
{
    public PasswordHasherMock SetupHash(string returnValue = "hashed_password")
    {
        _mock.Setup(x => x.Hash(It.IsAny<string>()))
             .Returns(returnValue);

        return this;
    }

    public PasswordHasherMock VerifyHashCalled(Times times)
    {
        _mock.Verify(x => x.Hash(It.IsAny<string>()), times);
        return this;
    }

    public PasswordHasherMock SetupVerify(bool returnValue)
    {
        _mock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
             .Returns(returnValue);

        return this;
    }

    public PasswordHasherMock VerifyVerifyCalled(Times times)
    {
        _mock.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), times);
        return this;
    }
}
