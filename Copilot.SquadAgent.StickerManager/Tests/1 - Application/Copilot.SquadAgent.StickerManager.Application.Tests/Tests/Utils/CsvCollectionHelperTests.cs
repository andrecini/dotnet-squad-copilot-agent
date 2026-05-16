using System.Text;
using Copilot.SquadAgent.StickerManager.Application.Utils;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.Tests.Utils;

public class CsvCollectionHelperTests
{
    [Fact]
    public void ParseCsvRows_ValidCsvWithTwoRows_ReturnsTwoRecords()
    {
        // Arrange
        var csv = "sticker_number,quantity\nBRA01,1\nBRA02,2";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows.Count.ShouldBe(2);
    }

    [Fact]
    public void ParseCsvRows_ValidCsvFirstRow_MapsFieldsCorrectly()
    {
        // Arrange
        var csv = "sticker_number,quantity\nBRA01,3";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows[0].StickerNumber.ShouldBe("BRA01");
        rows[0].Quantity.ShouldBe(3);
    }

    [Fact]
    public void ParseCsvRows_CsvWithHeaderOnly_ReturnsEmptyList()
    {
        // Arrange
        var csv = "sticker_number,quantity\n";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows.ShouldBeEmpty();
    }

    [Fact]
    public void ParseCsvRows_CsvMissingQuantityColumn_UsesDefaultValue()
    {
        // Arrange
        var csv = "sticker_number\nBRA01";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows.Count.ShouldBe(1);
        rows[0].Quantity.ShouldBe(1);
    }

    [Fact]
    public void ParseCsvRows_CsvWithWindowsLineEndings_ParsesCorrectly()
    {
        // Arrange
        var csv = "sticker_number,quantity\r\nBRA01,2\r\nBRA02,5\r\n";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows.Count.ShouldBe(2);
    }

    [Fact]
    public void ParseCsvRows_CsvWithMultipleRows_MapsAllStickerNumbers()
    {
        // Arrange
        var csv = "sticker_number,quantity\nBRA01,1\nBRA02,2\nBRA03,3";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows.Select(r => r.StickerNumber).ShouldBe(["BRA01", "BRA02", "BRA03"]);
    }

    [Fact]
    public void ParseCsvRows_CsvWithExtraColumns_IgnoresExtraColumnsAndParses()
    {
        // Arrange — MissingFieldFound=null; campos extras são ignorados pelo CsvHelper
        var csv = "sticker_number,quantity,extra_column\nBRA01,2,ignorar";
        using var stream = ToStream(csv);

        // Act
        var rows = CsvCollectionHelper.ParseCsvRows(stream);

        // Assert
        rows.Count.ShouldBe(1);
        rows[0].StickerNumber.ShouldBe("BRA01");
        rows[0].Quantity.ShouldBe(2);
    }

    [Fact]
    public void ParseCsvRows_CsvWithNullStream_ThrowsException()
    {
        // Arrange
        Stream stream = null!;

        // Act & Assert
        Should.Throw<Exception>(() => CsvCollectionHelper.ParseCsvRows(stream));
    }

    [Fact]
    public void GetTemplateContent_ReturnsNonEmptyBytes()
    {
        // Act
        var bytes = CsvCollectionHelper.GetTemplateContent();

        // Assert
        bytes.ShouldNotBeEmpty();
    }

    [Fact]
    public void GetTemplateContent_ContentContainsStickerNumberHeader()
    {
        // Act
        var bytes = CsvCollectionHelper.GetTemplateContent();
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        content.ShouldContain("sticker_number");
    }

    [Fact]
    public void GetTemplateContent_ContentContainsQuantityHeader()
    {
        // Act
        var bytes = CsvCollectionHelper.GetTemplateContent();
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        content.ShouldContain("quantity");
    }

    [Fact]
    public void GetTemplateContent_ContentContainsExampleRow()
    {
        // Act
        var bytes = CsvCollectionHelper.GetTemplateContent();
        var content = Encoding.UTF8.GetString(bytes);
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Assert
        lines.Length.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void GetTemplateContent_ReturnedBytesAreValidUtf8()
    {
        // Act
        var bytes = CsvCollectionHelper.GetTemplateContent();

        // Assert — não deve lançar exceção
        var content = Encoding.UTF8.GetString(bytes);
        content.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetTemplateContent_CalledTwice_ReturnsSameContent()
    {
        // Act
        var first = CsvCollectionHelper.GetTemplateContent();
        var second = CsvCollectionHelper.GetTemplateContent();

        // Assert
        first.ShouldBe(second);
    }

    private static Stream ToStream(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }
}
