using Copilot.SquadAgent.StickerManager.Integration.Tests.Fixtures;
using Copilot.SquadAgent.StickerManager.Integration.Tests.Helpers;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Tests.Collection;

[ExcludeFromCodeCoverage]
public class DownloadImportTemplateEndpointTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task DownloadImportTemplateAsync_AuthenticatedUser_ReturnsOkAsync()
    {
        // Arrange
        var email = $"template_ok_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/import/template");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_WithoutToken_ReturnsUnauthorizedAsync()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/collection/import/template");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_AuthenticatedUser_ReturnsCsvContentTypeAsync()
    {
        // Arrange
        var email = $"template_ct_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/import/template");

        // Assert
        response.Content.Headers.ContentType?.MediaType.ShouldBe("text/csv");
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_AuthenticatedUser_ReturnsCsvWithExpectedHeadersAsync()
    {
        // Arrange
        var email = $"template_hdr_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/import/template");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        content.ShouldContain("sticker_number");
        content.ShouldContain("quantity");
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_AuthenticatedUser_ReturnsNonEmptyBodyAsync()
    {
        // Arrange
        var email = $"template_body_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/import/template");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        // Assert
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task DownloadImportTemplateAsync_AuthenticatedUser_ResponseContainsExampleRowAsync()
    {
        // Arrange
        var email = $"template_row_{Guid.NewGuid():N}@test.com";
        var token = await AuthHelper.GetTokenAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/collection/import/template");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var content = Encoding.UTF8.GetString(bytes);
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Assert
        lines.Length.ShouldBeGreaterThanOrEqualTo(2);
    }
}
