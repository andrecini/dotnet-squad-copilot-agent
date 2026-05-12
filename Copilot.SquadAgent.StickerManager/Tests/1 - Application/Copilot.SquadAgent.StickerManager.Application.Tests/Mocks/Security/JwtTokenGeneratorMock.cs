using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Moq;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Security;

public class JwtTokenGeneratorMock : BaseMock<IJwtTokenGenerator>
{
    public JwtTokenGeneratorMock SetupGenerate(TokenModel returnValue)
    {
        _mock.Setup(x => x.Generate(It.IsAny<UserModel>()))
             .Returns(returnValue);

        return this;
    }

    public JwtTokenGeneratorMock VerifyGenerateCalled(Times times)
    {
        _mock.Verify(x => x.Generate(It.IsAny<UserModel>()), times);
        return this;
    }
}
