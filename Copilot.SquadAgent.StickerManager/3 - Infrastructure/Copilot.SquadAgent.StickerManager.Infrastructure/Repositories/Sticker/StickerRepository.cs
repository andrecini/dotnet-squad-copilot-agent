using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;

public class StickerRepository(AppDbContext dbContext) : IStickerRepository
{
    public async Task<Result<StickerEntity>> GetByIdAsync(Guid stickerId, CancellationToken cancellationToken)
    {
        var sticker = await dbContext.Stickers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == stickerId, cancellationToken);

        if (sticker is null)
            return Result<StickerEntity>.Failure(ResultCode.NotFound, "Figurinha não encontrada.", statusCode: 404);

        return Result<StickerEntity>.Success(sticker);
    }

    public async Task<Result<IReadOnlyList<MissingStickerItemModel>>> ListMissingByUserAsync(MissingStickersModel filter, CancellationToken cancellationToken)
    {
        var ownedStickerIds = dbContext.UserCollections
            .Where(uc => uc.UserId == filter.UserId && uc.DeletedAt == null)
            .Select(uc => uc.StickerId);

        var query = dbContext.Stickers
            .AsNoTracking()
            .Include(s => s.Team)
            .Where(s => !ownedStickerIds.Contains(s.Id))
            .AsQueryable();

        query = filter.Sort switch
        {
            "team"   => query.OrderBy(s => s.Team.Name).ThenBy(s => s.Code),
            "number" => query.OrderBy(s => s.Code),
            _        => query.OrderByDescending(s => s.Rarity).ThenBy(s => s.Team.Name).ThenBy(s => s.Code)
        };

        var items = await query
            .Skip((filter.Page - 1) * filter.Limit)
            .Take(filter.Limit)
            .Select(s => new MissingStickerItemModel
            {
                StickerId  = s.Id,
                Code       = s.Code,
                PlayerName = s.PlayerName,
                TeamName   = s.Team.Name,
                TeamCode   = s.Team.Code,
                Rarity     = s.Rarity
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<MissingStickerItemModel>>.Success(items);
    }

    public async Task<Result<IReadOnlyList<AlbumStickerModel>>> GetAlbumAsync(Guid userId, CancellationToken cancellationToken)
    {
        var items = await dbContext.Stickers
            .AsNoTracking()
            .Include(s => s.Team)
            .OrderBy(s => s.Team.Name)
            .ThenBy(s => s.Code)
            .Select(s => new AlbumStickerModel
            {
                StickerId         = s.Id,
                Code              = s.Code,
                PlayerName        = s.PlayerName,
                TeamName          = s.Team.Name,
                TeamCode          = s.Team.Code,
                Rarity            = s.Rarity,
                Owned             = s.UserCollections.Any(uc => uc.UserId == userId && uc.DeletedAt == null),
                QuantityOwned     = s.UserCollections
                                      .Where(uc => uc.UserId == userId && uc.DeletedAt == null)
                                      .Select(uc => uc.QuantityOwned)
                                      .FirstOrDefault(),
                QuantityDuplicate = s.UserCollections
                                      .Where(uc => uc.UserId == userId && uc.DeletedAt == null)
                                      .Select(uc => uc.QuantityDuplicate)
                                      .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AlbumStickerModel>>.Success(items);
    }
}
