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

    public CollectionServiceMock SetupRemoveStickerFromCollectionAsync(Result returnValue)
    {
        _mock.Setup(x => x.RemoveStickerFromCollectionAsync(It.IsAny<RemoveStickerFromCollectionModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock SetupToggleDuplicateAsync(Result<UserCollectionModel> returnValue)
    {
        _mock.Setup(x => x.ToggleDuplicateAsync(It.IsAny<ToggleDuplicateModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock VerifyAddStickerAsyncCalled(Times times)
    {
        _mock.Verify(x => x.AddStickerAsync(It.IsAny<AddToCollectionModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public CollectionServiceMock VerifyRemoveStickerFromCollectionAsyncCalled(Times times)
    {
        _mock.Verify(x => x.RemoveStickerFromCollectionAsync(It.IsAny<RemoveStickerFromCollectionModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public CollectionServiceMock VerifyToggleDuplicateAsyncCalled(Times times)
    {
        _mock.Verify(x => x.ToggleDuplicateAsync(It.IsAny<ToggleDuplicateModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
