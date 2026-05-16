using System.Globalization;
using AutoMapper;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Api.Mappings;
using CsvHelper;
using CsvHelper.Configuration;

namespace Copilot.SquadAgent.StickerManager.Api.AppServices;


public class CollectionAppService(ICollectionService collectionService, IMapper mapper) : ICollectionAppService
{
    public async Task<IResult> AddStickerAsync(Guid userId, AddToCollectionRequest request, CancellationToken cancellationToken)
    {
        var model = mapper.Map<AddToCollectionModel>(request);
        model.UserId = userId;

        var result = await collectionService.AddStickerAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<AddToCollectionResponse>(result.Value);
        return TypedResults.Created($"/api/v1/collection/{response.CollectionId}", response);
    }

    public async Task<IResult> RemoveStickerFromCollectionAsync(Guid userId, Guid collectionId, CancellationToken cancellationToken)
    {
        var model = new RemoveStickerFromCollectionModel
        {
            CollectionId = collectionId,
            UserId = userId
        };

        var result = await collectionService.RemoveStickerFromCollectionAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        return TypedResults.NoContent();
    }

    public async Task<IResult> ToggleDuplicateAsync(Guid userId, Guid collectionId, ToggleDuplicateRequest request, CancellationToken cancellationToken)
    {
        var model = new ToggleDuplicateModel
        {
            CollectionId = collectionId,
            UserId = userId,
            Action = request.Action
        };

        var result = await collectionService.ToggleDuplicateAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<ToggleDuplicateResponse>(result.Value);
        return TypedResults.Ok(response);
    }

    public async Task<IResult> ListCollectionAsync(Guid userId, CollectionQueryRequest query, CancellationToken cancellationToken)
    {
        var model = mapper.Map<ListCollectionModel>(query);
        model.UserId = userId;

        var result = await collectionService.ListCollectionAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<IReadOnlyList<CollectionItemResponse>>(result.Value);
        return TypedResults.Ok(response);
    }

    public async Task<IResult> ListMissingStickersAsync(Guid userId, MissingStickersQueryRequest query, CancellationToken cancellationToken)
    {
        var model = mapper.Map<MissingStickersModel>(query);
        model.UserId = userId;

        var result = await collectionService.ListMissingStickersAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<IReadOnlyList<MissingStickerItemResponse>>(result.Value);
        return TypedResults.Ok(response);
    }

    public async Task<IResult> GetCollectionStatsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await collectionService.GetCollectionStatsAsync(userId, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<CollectionStatsResponse>(result.Value);
        return TypedResults.Ok(response);
    }

    public async Task<IResult> ImportCollectionAsync(Guid userId, ImportCollectionRequest request, CancellationToken cancellationToken)
    {
        List<CsvStickerRowModel> rows;

        try
        {
            rows = ParseCsvRows(request.File);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                $"Erro ao processar o arquivo CSV: {ex.Message}",
                statusCode: 400);
        }

        var model = new ImportCollectionModel
        {
            UserId = userId,
            Rows = rows
        };

        var result = await collectionService.ImportCollectionAsync(model, cancellationToken);

        if (result.IsFailure)
            return Results.Problem(result.Message, statusCode: result.StatusCode ?? 500);

        var response = mapper.Map<ImportCollectionResponse>(result.Value);
        return TypedResults.Ok(response);
    }

    public Task<IResult> DownloadImportTemplateAsync(CancellationToken cancellationToken)
    {
        const string csvContent = "sticker_number,quantity\r\n1,2\r\n";

        var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
        var result = TypedResults.File(
            bytes,
            contentType: "text/csv",
            fileDownloadName: "template-importacao-colecao.csv");

        return Task.FromResult<IResult>(result);
    }

    private static List<CsvStickerRowModel> ParseCsvRows(IFormFile file)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<CsvStickerRowMap>();

        return [.. csv.GetRecords<CsvStickerRowModel>()];
    }
}
