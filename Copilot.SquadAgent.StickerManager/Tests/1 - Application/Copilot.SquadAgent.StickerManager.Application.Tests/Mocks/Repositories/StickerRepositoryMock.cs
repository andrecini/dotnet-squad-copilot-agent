using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Mocks.Repositories;

public class StickerRepositoryMock : BaseMock<IStickerRepository>
{
    public StickerRepositoryMock SetupGetByIdAsync(Result<StickerEntity> returnValue)
    {
        _mock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }
}
