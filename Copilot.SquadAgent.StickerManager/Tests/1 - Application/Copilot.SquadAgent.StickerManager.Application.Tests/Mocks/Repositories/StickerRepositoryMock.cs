using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
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

    public StickerRepositoryMock SetupListMissingByUserAsync(Result<IReadOnlyList<MissingStickerItemModel>> returnValue)
    {
        _mock.Setup(x => x.ListMissingByUserAsync(It.IsAny<MissingStickersModel>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public StickerRepositoryMock VerifyListMissingByUserAsyncCalled(Times times)
    {
        _mock.Verify(x => x.ListMissingByUserAsync(It.IsAny<MissingStickersModel>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    public StickerRepositoryMock SetupGetAlbumAsync(Result<IReadOnlyList<AlbumStickerModel>> returnValue)
    {
        _mock.Setup(x => x.GetAlbumAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(returnValue);

        return this;
    }

    public StickerRepositoryMock VerifyGetAlbumAsyncCalled(Times times)
    {
        _mock.Verify(x => x.GetAlbumAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}
