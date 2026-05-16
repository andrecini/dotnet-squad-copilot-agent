using System.Globalization;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Copilot.SquadAgent.StickerManager.Application.Utils;

public static class CsvCollectionHelper
{
    private const string TemplateContent = "sticker_number,quantity\r\n1,2\r\n";

    public static List<CsvStickerRowModel> ParseCsvRows(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<CsvStickerRowClassMap>();

        return [.. csv.GetRecords<CsvStickerRowModel>()];
    }

    public static byte[] GetTemplateContent()
    {
        return System.Text.Encoding.UTF8.GetBytes(TemplateContent);
    }

    private sealed class CsvStickerRowClassMap : ClassMap<CsvStickerRowModel>
    {
        public CsvStickerRowClassMap()
        {
            Map(m => m.StickerNumber).Name("sticker_number");
            Map(m => m.Quantity).Name("quantity").Default(1);
        }
    }
}
