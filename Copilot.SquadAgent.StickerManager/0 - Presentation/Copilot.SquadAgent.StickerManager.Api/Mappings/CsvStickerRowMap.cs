using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using CsvHelper.Configuration;

namespace Copilot.SquadAgent.StickerManager.Api.Mappings;

[ExcludeFromCodeCoverage]
public class CsvStickerRowMap : ClassMap<CsvStickerRowModel>
{
    public CsvStickerRowMap()
    {
        Map(m => m.StickerNumber).Name("sticker_number");
        Map(m => m.Quantity).Name("quantity").Default(1);
    }
}
