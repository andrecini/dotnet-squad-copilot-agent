using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class ImportCollectionModelMock
{
    public static ImportCollectionModel Valid() => new()
    {
        UserId = Guid.NewGuid(),
        Rows =
        [
            new CsvStickerRowModel { StickerNumber = "BRA01", Quantity = 1 },
            new CsvStickerRowModel { StickerNumber = "BRA02", Quantity = 2 }
        ]
    };

    public static ImportCollectionModel WithSingleRow(Guid userId, string stickerNumber, int quantity = 1) => new()
    {
        UserId = userId,
        Rows = [new CsvStickerRowModel { StickerNumber = stickerNumber, Quantity = quantity }]
    };

    public static ImportCollectionModel WithEmptyRows() => new()
    {
        UserId = Guid.NewGuid(),
        Rows = []
    };

    public static ImportCollectionModel ExceedingMaxLines() => new()
    {
        UserId = Guid.NewGuid(),
        Rows = Enumerable.Range(1, 1001)
            .Select(i => new CsvStickerRowModel { StickerNumber = $"CODE{i:D4}", Quantity = 1 })
            .ToList()
    };

    public static ImportCollectionModel WithInvalidRow() => new()
    {
        UserId = Guid.NewGuid(),
        Rows =
        [
            new CsvStickerRowModel { StickerNumber = "", Quantity = 1 },
            new CsvStickerRowModel { StickerNumber = "BRA01", Quantity = 0 }
        ]
    };
}
