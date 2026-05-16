using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;

public static class ImportCollectionResultModelMock
{
    public static ImportCollectionResultModel Valid() => new()
    {
        Imported = 5,
        Failed = 0,
        Errors = []
    };

    public static ImportCollectionResultModel WithErrors() => new()
    {
        Imported = 3,
        Failed = 2,
        Errors =
        [
            new ImportCollectionErrorModel { Line = 2, Reason = "Figurinha com código 'INVALID' não encontrada." },
            new ImportCollectionErrorModel { Line = 4, Reason = "O campo quantity deve ser maior que zero." }
        ]
    };
}
