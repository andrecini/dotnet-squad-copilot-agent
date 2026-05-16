using AutoMapper;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Application.Services.Collection;

public class CollectionService(
    IStickerRepository stickerRepository,
    IUserCollectionRepository userCollectionRepository,
    IMapper mapper) : ICollectionService
{
    private const int MaxDuplicates = 10;

    public async Task<Result> RemoveStickerFromCollectionAsync(RemoveStickerFromCollectionModel model, CancellationToken cancellationToken)
    {
        var getResult = await userCollectionRepository.GetByIdAsync(model.CollectionId, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure(getResult.Code, getResult.Message!, getResult.StatusCode);

        var entry = getResult.Value;

        if (entry is null)
            return Result.Failure(ResultCode.NotFound, "Registro de coleção não encontrado.", 404);

        if (entry.UserId != model.UserId)
            return Result.Failure(ResultCode.Forbidden, "Você não tem permissão para remover este registro.", 403);

        var deleteResult = await userCollectionRepository.SoftDeleteAsync(entry, cancellationToken);

        if (deleteResult.IsFailure)
            return Result.Failure(deleteResult.Code, deleteResult.Message!, deleteResult.StatusCode);

        return Result.Success();
    }

    public async Task<Result<UserCollectionModel>> AddStickerAsync(AddToCollectionModel model, CancellationToken cancellationToken)
    {
        var stickerResult = await stickerRepository.GetByIdAsync(model.StickerId, cancellationToken);

        if (stickerResult.IsFailure)
            return Result<UserCollectionModel>.Failure(stickerResult.Code, stickerResult.Message!, stickerResult.StatusCode);

        var existingResult = await userCollectionRepository.GetByUserAndStickerAsync(model.UserId, model.StickerId, cancellationToken);

        if (existingResult.IsFailure)
            return Result<UserCollectionModel>.Failure(existingResult.Code, existingResult.Message!, existingResult.StatusCode);

        var existing = existingResult.Value;

        if (existing is not null)
        {
            if (existing.QuantityDuplicate >= MaxDuplicates)
                return Result<UserCollectionModel>.Failure(
                    ResultCode.BusinessError,
                    $"Limite máximo de {MaxDuplicates} duplicatas atingido para esta figurinha.",
                    statusCode: 422);

            existing.QuantityOwned++;
            existing.QuantityDuplicate++;

            var updateResult = await userCollectionRepository.UpdateAsync(existing, cancellationToken);

            if (updateResult.IsFailure)
                return Result<UserCollectionModel>.Failure(updateResult.Code, updateResult.Message!, updateResult.StatusCode);

            return Result<UserCollectionModel>.Success(mapper.Map<UserCollectionModel>(updateResult.Value));
        }

        var newEntry = new UserCollection
        {
            UserId = model.UserId,
            StickerId = model.StickerId,
            QuantityOwned = 1,
            QuantityDuplicate = 0
        };

        var createResult = await userCollectionRepository.CreateAsync(newEntry, cancellationToken);

        if (createResult.IsFailure)
            return Result<UserCollectionModel>.Failure(createResult.Code, createResult.Message!, createResult.StatusCode);

        return Result<UserCollectionModel>.Success(mapper.Map<UserCollectionModel>(createResult.Value));
    }

    public async Task<Result<UserCollectionModel>> ToggleDuplicateAsync(ToggleDuplicateModel model, CancellationToken cancellationToken)
    {
        var getResult = await userCollectionRepository.GetByIdAsync(model.CollectionId, cancellationToken);

        if (getResult.IsFailure)
            return Result<UserCollectionModel>.Failure(getResult.Code, getResult.Message!, getResult.StatusCode);

        var entry = getResult.Value;

        if (entry is null)
            return Result<UserCollectionModel>.Failure(ResultCode.NotFound, "Registro de coleção não encontrado.", 404);

        if (entry.UserId != model.UserId)
            return Result<UserCollectionModel>.Failure(ResultCode.Forbidden, "Você não tem permissão para alterar este registro.", 403);

        if (model.Action == DuplicateAction.Mark)
        {
            if (entry.QuantityDuplicate >= entry.QuantityOwned)
                return Result<UserCollectionModel>.Failure(
                    ResultCode.BusinessError,
                    "Todas as figurinhas já estão marcadas como duplicata.",
                    statusCode: 422);

            entry.QuantityDuplicate++;
        }
        else
        {
            if (entry.QuantityDuplicate <= 0)
                return Result<UserCollectionModel>.Failure(
                    ResultCode.BusinessError,
                    "Não há duplicatas para desmarcar.",
                    statusCode: 422);

            entry.QuantityDuplicate--;
        }

        var updateResult = await userCollectionRepository.UpdateAsync(entry, cancellationToken);

        if (updateResult.IsFailure)
            return Result<UserCollectionModel>.Failure(updateResult.Code, updateResult.Message!, updateResult.StatusCode);

        return Result<UserCollectionModel>.Success(mapper.Map<UserCollectionModel>(updateResult.Value));
    }

    public async Task<Result<IReadOnlyList<CollectionItemModel>>> ListCollectionAsync(ListCollectionModel model, CancellationToken cancellationToken)
    {
        var result = await userCollectionRepository.ListByUserAsync(model, cancellationToken);

        if (result.IsFailure)
            return Result<IReadOnlyList<CollectionItemModel>>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<IReadOnlyList<CollectionItemModel>>.Success(result.Value!);
    }

    public async Task<Result<IReadOnlyList<MissingStickerItemModel>>> ListMissingStickersAsync(MissingStickersModel model, CancellationToken cancellationToken)
    {
        var result = await stickerRepository.ListMissingByUserAsync(model, cancellationToken);

        if (result.IsFailure)
            return Result<IReadOnlyList<MissingStickerItemModel>>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<IReadOnlyList<MissingStickerItemModel>>.Success(result.Value!);
    }

    public async Task<Result<CollectionStatsModel>> GetCollectionStatsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await userCollectionRepository.GetStatsAsync(userId, cancellationToken);

        if (result.IsFailure)
            return Result<CollectionStatsModel>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<CollectionStatsModel>.Success(result.Value!);
    }

    public async Task<Result<ImportCollectionResultModel>> ImportCollectionAsync(ImportCollectionModel model, CancellationToken cancellationToken)
    {
        const int MaxLines = 1000;

        if (model.Rows.Count > MaxLines)
            return Result<ImportCollectionResultModel>.Failure(
                ResultCode.BusinessError,
                $"O arquivo CSV não pode ter mais de {MaxLines} linhas.",
                statusCode: 422);

        var errors = new List<ImportCollectionErrorModel>();
        var entriesToUpsert = new List<UserCollection>();
        var lineNumber = 2; // linha 1 é o cabeçalho

        foreach (var row in model.Rows)
        {
            if (string.IsNullOrWhiteSpace(row.StickerNumber))
            {
                errors.Add(new ImportCollectionErrorModel { Line = lineNumber, Reason = "O campo sticker_number é obrigatório." });
                lineNumber++;
                continue;
            }

            if (row.Quantity <= 0)
            {
                errors.Add(new ImportCollectionErrorModel { Line = lineNumber, Reason = "O campo quantity deve ser maior que zero." });
                lineNumber++;
                continue;
            }

            var stickerResult = await stickerRepository.GetByCodeAsync(row.StickerNumber, cancellationToken);

            if (stickerResult.IsFailure)
            {
                errors.Add(new ImportCollectionErrorModel { Line = lineNumber, Reason = $"Erro ao buscar figurinha: {stickerResult.Message}" });
                lineNumber++;
                continue;
            }

            if (stickerResult.Value is null)
            {
                errors.Add(new ImportCollectionErrorModel { Line = lineNumber, Reason = $"Figurinha com código '{row.StickerNumber}' não encontrada." });
                lineNumber++;
                continue;
            }

            var sticker = stickerResult.Value;

            var existingResult = await userCollectionRepository.GetByUserAndStickerAsync(model.UserId, sticker.Id, cancellationToken);

            if (existingResult.IsFailure)
            {
                errors.Add(new ImportCollectionErrorModel { Line = lineNumber, Reason = $"Erro ao verificar coleção: {existingResult.Message}" });
                lineNumber++;
                continue;
            }

            if (existingResult.Value is not null)
            {
                existingResult.Value.QuantityOwned += row.Quantity;
                entriesToUpsert.Add(existingResult.Value);
            }
            else
            {
                entriesToUpsert.Add(new UserCollection
                {
                    UserId = model.UserId,
                    StickerId = sticker.Id,
                    QuantityOwned = row.Quantity,
                    QuantityDuplicate = 0
                });
            }

            lineNumber++;
        }

        if (entriesToUpsert.Count == 0)
        {
            return Result<ImportCollectionResultModel>.Success(new ImportCollectionResultModel
            {
                Imported = 0,
                Failed = errors.Count,
                Errors = errors
            });
        }

        var bulkResult = await userCollectionRepository.BulkUpsertAsync(entriesToUpsert, cancellationToken);

        if (bulkResult.IsFailure)
            return Result<ImportCollectionResultModel>.Failure(bulkResult.Code, bulkResult.Message!, bulkResult.StatusCode);

        return Result<ImportCollectionResultModel>.Success(new ImportCollectionResultModel
        {
            Imported = bulkResult.Value,
            Failed = errors.Count,
            Errors = errors
        });
    }
}
