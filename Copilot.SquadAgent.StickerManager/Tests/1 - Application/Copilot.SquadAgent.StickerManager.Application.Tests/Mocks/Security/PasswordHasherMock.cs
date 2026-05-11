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
}
