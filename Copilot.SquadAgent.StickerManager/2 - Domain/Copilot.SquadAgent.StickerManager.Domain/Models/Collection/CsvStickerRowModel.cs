using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class CsvStickerRowModel
{
    public string StickerNumber { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}
