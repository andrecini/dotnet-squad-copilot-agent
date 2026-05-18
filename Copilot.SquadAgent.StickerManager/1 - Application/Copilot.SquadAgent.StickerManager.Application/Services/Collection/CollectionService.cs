using AutoMapper;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using Copilot.SquadAgent.StickerManager.Domain.Models;
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

    public async Task<Result<PagedResult<AlbumStickerModel>>> GetAlbumAsync(AlbumQueryModel query, CancellationToken cancellationToken)
    {
        var result = await stickerRepository.GetAlbumAsync(query, cancellationToken);

        if (result.IsFailure)
            return Result<PagedResult<AlbumStickerModel>>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<PagedResult<AlbumStickerModel>>.Success(result.Value!);
    }

    public async Task<Result<TeamProgressModel>> GetTeamProgressAsync(Guid teamId, Guid userId, CancellationToken cancellationToken)
    {
        var result = await stickerRepository.GetTeamProgressAsync(teamId, userId, cancellationToken);

        if (result.IsFailure)
            return Result<TeamProgressModel>.Failure(result.Code, result.Message!, result.StatusCode);

        return Result<TeamProgressModel>.Success(result.Value!);
    }
}
