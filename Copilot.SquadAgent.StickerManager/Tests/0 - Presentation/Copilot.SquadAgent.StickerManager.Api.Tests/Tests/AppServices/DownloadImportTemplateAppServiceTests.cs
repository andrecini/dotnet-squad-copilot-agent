using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using Copilot.SquadAgent.StickerManager.Api.Tests.Mocks.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.AppServices;

public class DownloadImportTemplateAppServiceTests
{
    private readonly IMapper _mapper;

    public DownloadImportTemplateAppServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectionProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_ReturnsFileContentResultAsync()
    {
        // Arrange
        var collectionService = new CollectionServiceMock().Build();
        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.DownloadImportTemplateAsync(CancellationToken.None);

        // Assert
        result.ShouldBeOfType<FileContentHttpResult>();
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_ReturnsCsvContentTypeAsync()
    {
        // Arrange
        var collectionService = new CollectionServiceMock().Build();
        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.DownloadImportTemplateAsync(CancellationToken.None);

        // Assert
        var file = (FileContentHttpResult)result;
        file.ContentType.ShouldBe("text/csv");
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_ReturnsCorrectFileDownloadNameAsync()
    {
        // Arrange
        var collectionService = new CollectionServiceMock().Build();
        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.DownloadImportTemplateAsync(CancellationToken.None);

        // Assert
        var file = (FileContentHttpResult)result;
        file.FileDownloadName.ShouldBe("template-importacao-colecao.csv");
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_CsvContainsExpectedHeaderAsync()
    {
        // Arrange
        var collectionService = new CollectionServiceMock().Build();
        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.DownloadImportTemplateAsync(CancellationToken.None);

        // Assert
        var file = (FileContentHttpResult)result;
        var csvContent = System.Text.Encoding.UTF8.GetString(file.FileContents.ToArray());
        csvContent.ShouldContain("sticker_number");
        csvContent.ShouldContain("quantity");
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_CsvContainsExampleRowAsync()
    {
        // Arrange
        var collectionService = new CollectionServiceMock().Build();
        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.DownloadImportTemplateAsync(CancellationToken.None);

        // Assert
        var file = (FileContentHttpResult)result;
        var csvContent = System.Text.Encoding.UTF8.GetString(file.FileContents.ToArray());
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Length.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_FileContentIsNotEmptyAsync()
    {
        // Arrange
        var collectionService = new CollectionServiceMock().Build();
        var appService = new CollectionAppService(collectionService, _mapper);

        // Act
        var result = await appService.DownloadImportTemplateAsync(CancellationToken.None);

        // Assert
        var file = (FileContentHttpResult)result;
        file.FileContents.Length.ShouldBeGreaterThan(0);
    }
}
