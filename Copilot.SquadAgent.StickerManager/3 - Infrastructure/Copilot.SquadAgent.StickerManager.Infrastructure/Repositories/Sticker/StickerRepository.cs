using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models;
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
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
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

    public async Task<Result<PagedResult<AlbumStickerModel>>> GetAlbumAsync(AlbumQueryModel query, CancellationToken cancellationToken)
    {
        var userId = query.UserId;

        var baseQuery = dbContext.Stickers
            .AsNoTracking()
            .Include(s => s.Team)
            .AsQueryable();

        if (query.TeamId.HasValue)
            baseQuery = baseQuery.Where(s => s.TeamId == query.TeamId.Value);

        baseQuery = query.SortByTeam
            ? baseQuery.OrderBy(s => s.Team.Name).ThenBy(s => s.Code)
            : baseQuery.OrderBy(s => s.Code);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
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

        var paged = new PagedResult<AlbumStickerModel>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = query.Page,
            PageSize   = query.PageSize
        };

        return Result<PagedResult<AlbumStickerModel>>.Success(paged);
    }

    public async Task<Result<TeamProgressModel>> GetTeamProgressAsync(Guid teamId, Guid userId, CancellationToken cancellationToken)
    {
        var team = await dbContext.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

        if (team is null)
            return Result<TeamProgressModel>.Failure(ResultCode.NotFound, "Time não encontrado.", statusCode: 404);

        var totalStickers = await dbContext.Stickers
            .AsNoTracking()
            .CountAsync(s => s.TeamId == teamId, cancellationToken);

        var ownedCount = await dbContext.UserCollections
            .AsNoTracking()
            .CountAsync(
                uc => uc.UserId == userId
                   && uc.DeletedAt == null
                   && uc.Sticker.TeamId == teamId,
                cancellationToken);

        var completionPercentage = totalStickers == 0
            ? 0.0
            : Math.Round((double)ownedCount / totalStickers * 100, 1);

        var model = new TeamProgressModel
        {
            TeamId               = team.Id,
            TeamName             = team.Name,
            TotalStickers        = totalStickers,
            OwnedCount           = ownedCount,
            CompletionPercentage = completionPercentage
        };

        return Result<TeamProgressModel>.Success(model);
    }
}
