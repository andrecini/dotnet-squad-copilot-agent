using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class ImportCollectionResultModelMock
{
    public static ImportCollectionResultModel Valid() => new()
    {
        Imported = 2,
        Failed = 0,
        Errors = []
    };

    public static ImportCollectionResultModel WithErrors() => new()
    {
        Imported = 1,
        Failed = 1,
        Errors =
        [
            new ImportCollectionErrorModel { Line = 2, Reason = "Figurinha com código 'INVALID' não encontrada." }
        ]
    };

    public static ImportCollectionResultModel AllFailed() => new()
    {
        Imported = 0,
        Failed = 2,
        Errors =
        [
            new ImportCollectionErrorModel { Line = 2, Reason = "Figurinha com código 'INVALID1' não encontrada." },
            new ImportCollectionErrorModel { Line = 3, Reason = "Figurinha com código 'INVALID2' não encontrada." }
        ]
    };
}
