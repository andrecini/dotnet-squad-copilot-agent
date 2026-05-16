using System.Text;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;
using Copilot.SquadAgent.StickerManager.Api.Validators.Collection;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.Tests.Validators;

public class ImportCollectionRequestValidatorTests
{
    private readonly ImportCollectionRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidCsvFile_PassesValidation()
    {
        // Arrange
        var request = ImportCollectionRequestMock.Valid();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_NullFile_FailsValidation()
    {
        // Arrange
        var request = new ImportCollectionRequest { File = null! };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(ImportCollectionRequest.File));
    }

    [Fact]
    public void Validate_NullFile_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var request = new ImportCollectionRequest { File = null! };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.ShouldContain(e => e.ErrorMessage == "O arquivo CSV é obrigatório.");
    }

    [Fact]
    public void Validate_EmptyFile_FailsValidation()
    {
        // Arrange
        var request = ImportCollectionRequestMock.WithEmptyFile();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "O arquivo CSV não pode estar vazio.");
    }

    [Fact]
    public void Validate_InvalidExtension_FailsValidation()
    {
        // Arrange
        var request = ImportCollectionRequestMock.WithInvalidExtension();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "Apenas arquivos CSV são aceitos.");
    }

    [Fact]
    public void Validate_FileTooLarge_FailsValidation()
    {
        // Arrange
        var largeContent = new string('a', 6 * 1024 * 1024); // 6MB
        var bytes = Encoding.UTF8.GetBytes(largeContent);
        var stream = new MemoryStream(bytes);
        var file = new FormFile(stream, 0, bytes.Length, "file", "large.csv")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };

        var request = new ImportCollectionRequest { File = file };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "O arquivo CSV não pode ultrapassar 5 MB.");
    }

    [Fact]
    public void Validate_ValidCsvContentType_PassesValidation()
    {
        // Arrange
        var content = "sticker_number,quantity\nBRA01,1";
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        var file = new FormFile(stream, 0, bytes.Length, "file", "import.csv")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };

        var request = new ImportCollectionRequest { File = file };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}
