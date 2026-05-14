using AutoMapper;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
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
}
