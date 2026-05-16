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

    public CollectionServiceMock SetupListCollectionAsync(Result<IReadOnlyList<CollectionItemModel>> returnValue)
    {
        _mock.Setup(x => x.ListCollectionAsync(It.IsAny<ListCollectionModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock VerifyListCollectionAsyncCalled(Times times)
    {
        _mock.Verify(x => x.ListCollectionAsync(It.IsAny<ListCollectionModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public CollectionServiceMock SetupListMissingStickersAsync(Result<IReadOnlyList<MissingStickerItemModel>> returnValue)
    {
        _mock.Setup(x => x.ListMissingStickersAsync(It.IsAny<MissingStickersModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock VerifyListMissingStickersAsyncCalled(Times times)
    {
        _mock.Verify(x => x.ListMissingStickersAsync(It.IsAny<MissingStickersModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public CollectionServiceMock SetupGetCollectionStatsAsync(Result<CollectionStatsModel> returnValue)
    {
        _mock.Setup(x => x.GetCollectionStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock VerifyGetCollectionStatsAsyncCalled(Times times)
    {
        _mock.Verify(x => x.GetCollectionStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public CollectionServiceMock SetupImportCollectionAsync(Result<ImportCollectionResultModel> returnValue)
    {
        _mock.Setup(x => x.ImportCollectionAsync(It.IsAny<ImportCollectionModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public CollectionServiceMock VerifyImportCollectionAsyncCalled(Times times)
    {
        _mock.Verify(x => x.ImportCollectionAsync(It.IsAny<ImportCollectionModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
