using Moq;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Mocks;

public class BaseMock<T> where T : class
{
    protected Mock<T> _mock = new();

    public T Build() => _mock.Object;
}
