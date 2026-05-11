using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data;

[ExcludeFromCodeCoverage]
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Sticker> Stickers { get; set; }
    public DbSet<UserCollection> UserCollections { get; set; }
    public DbSet<TradeOffer> TradeOffers { get; set; }
    public DbSet<TradeOfferItem> TradeOfferItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
