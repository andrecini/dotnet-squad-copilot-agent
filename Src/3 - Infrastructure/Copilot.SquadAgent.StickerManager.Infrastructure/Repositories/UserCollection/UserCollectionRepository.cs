using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.UserCollection;

public class UserCollectionRepository(AppDbContext dbContext) : IUserCollectionRepository
{
    public async Task<Result<UserCollectionEntity?>> GetByUserAndStickerAsync(Guid userId, Guid stickerId, CancellationToken cancellationToken)
    {
        var userCollection = await dbContext.UserCollections
            .FirstOrDefaultAsync(x => x.UserId == userId && x.StickerId == stickerId && x.DeletedAt == null, cancellationToken);

        return Result<UserCollectionEntity?>.Success(userCollection);
    }

    public async Task<Result<UserCollectionEntity?>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userCollection = await dbContext.UserCollections
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

        return Result<UserCollectionEntity?>.Success(userCollection);
    }

    public async Task<Result<UserCollectionEntity>> CreateAsync(UserCollectionEntity userCollection, CancellationToken cancellationToken)
    {
        await dbContext.UserCollections.AddAsync(userCollection, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserCollectionEntity>.Success(userCollection);
    }

    public async Task<Result<UserCollectionEntity>> UpdateAsync(UserCollectionEntity userCollection, CancellationToken cancellationToken)
    {
        dbContext.UserCollections.Update(userCollection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserCollectionEntity>.Success(userCollection);
    }

    public async Task<Result> SoftDeleteAsync(UserCollectionEntity userCollection, CancellationToken cancellationToken)
    {
        userCollection.DeletedAt = DateTime.UtcNow;
        dbContext.UserCollections.Update(userCollection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<CollectionItemModel>>> ListByUserAsync(ListCollectionModel filter, CancellationToken cancellationToken)
    {
        var query = dbContext.UserCollections
            .AsNoTracking()
            .Where(uc => uc.UserId == filter.UserId && uc.DeletedAt == null)
            .Include(uc => uc.Sticker)
                .ThenInclude(s => s.Team)
            .AsQueryable();

        if (filter.TeamId.HasValue)
            query = query.Where(uc => uc.Sticker.TeamId == filter.TeamId.Value);

        if (filter.Rarity.HasValue)
            query = query.Where(uc => uc.Sticker.Rarity == filter.Rarity.Value);

        query = filter.Sort switch
        {
            "player_name" => query.OrderBy(uc => uc.Sticker.PlayerName),
            "acquired_at"  => query.OrderBy(uc => uc.CreatedAt),
            _              => query.OrderBy(uc => uc.Sticker.Code)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(uc => new CollectionItemModel
            {
                StickerId         = uc.StickerId,
                PlayerName        = uc.Sticker.PlayerName,
                TeamName          = uc.Sticker.Team.Name,
                TeamCode          = uc.Sticker.Team.Code,
                Rarity            = uc.Sticker.Rarity,
                QuantityOwned     = uc.QuantityOwned,
                QuantityDuplicate = uc.QuantityDuplicate,
                AcquiredAt        = uc.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var paged = new PagedResult<CollectionItemModel>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = filter.Page,
            PageSize   = filter.PageSize
        };

        return Result<PagedResult<CollectionItemModel>>.Success(paged);
    }

    public async Task<Result<CollectionStatsModel>> GetStatsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var totalStickers = await dbContext.Stickers
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var userCollections = await dbContext.UserCollections
            .AsNoTracking()
            .Where(uc => uc.UserId == userId && uc.DeletedAt == null)
            .Include(uc => uc.Sticker)
                .ThenInclude(s => s.Team)
            .ToListAsync(cancellationToken);

        var totalOwned = userCollections.Count;
        var duplicates = userCollections.Sum(uc => uc.QuantityDuplicate);
        var totalMissing = totalStickers - totalOwned;
        var completionPercentage = totalStickers > 0
            ? Math.Round((double)totalOwned / totalStickers * 100, 1)
            : 0;

        var byTeam = userCollections
            .GroupBy(uc => uc.Sticker.Team)
            .Select(g => new TeamStatsModel
            {
                TeamId   = g.Key.Id,
                TeamName = g.Key.Name,
                TeamCode = g.Key.Code,
                Owned    = g.Count(),
                Total    = dbContext.Stickers.Count(s => s.TeamId == g.Key.Id),
                CompletionPercentage = dbContext.Stickers.Count(s => s.TeamId == g.Key.Id) > 0
                    ? Math.Round((double)g.Count() / dbContext.Stickers.Count(s => s.TeamId == g.Key.Id) * 100, 1)
                    : 0
            })
            .ToList();

        var stats = new CollectionStatsModel
        {
            TotalOwned            = totalOwned,
            TotalMissing          = totalMissing < 0 ? 0 : totalMissing,
            TotalStickers         = totalStickers,
            CompletionPercentage  = completionPercentage,
            Duplicates            = duplicates,
            ByTeam                = byTeam
        };

        return Result<CollectionStatsModel>.Success(stats);
    }
}
