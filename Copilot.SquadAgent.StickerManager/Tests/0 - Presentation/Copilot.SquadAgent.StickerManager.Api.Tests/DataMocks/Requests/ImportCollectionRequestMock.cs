using System.Text;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Microsoft.AspNetCore.Http;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class ImportCollectionRequestMock
{
    public static ImportCollectionRequest Valid() => new()
    {
        File = CreateFormFile("sticker_number,quantity\nBRA01,1\nBRA02,2", "import.csv", "text/csv")
    };

    public static ImportCollectionRequest WithEmptyFile() => new()
    {
        File = CreateFormFile("", "empty.csv", "text/csv")
    };

    public static ImportCollectionRequest WithInvalidExtension() => new()
    {
        File = CreateFormFile("some content", "import.txt", "text/plain")
    };

    private static IFormFile CreateFormFile(string content, string fileName, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
