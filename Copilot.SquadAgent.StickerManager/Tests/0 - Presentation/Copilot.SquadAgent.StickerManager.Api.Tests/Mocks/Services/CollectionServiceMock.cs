using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Moq;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;

public class CollectionServiceMock : BaseMock<ICollectionService>
{
    public CollectionServiceMock SetupAddStickerAsync(Result<UserCollectionModel> returnValue)
    {
        _mock.Setup(x => x.AddStickerAsync(It.IsAny<AddToCollectionModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock VerifyAddStickerAsyncCalled(Times times)
    {
        _mock.Verify(x => x.AddStickerAsync(It.IsAny<AddToCollectionModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
