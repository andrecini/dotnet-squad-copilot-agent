using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Configurations;

public class TradeOfferItemConfiguration : IEntityTypeConfiguration<TradeOfferItem>
{
    public void Configure(EntityTypeBuilder<TradeOfferItem> builder)
    {
        builder.ToTable("trade_offer_items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TradeOfferId)
            .HasColumnName("trade_offer_id")
            .IsRequired();

        builder.Property(x => x.StickerId)
            .HasColumnName("sticker_id")
            .IsRequired();

        builder.Property(x => x.Direction)
            .HasColumnName("direction")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(x => x.DeletedAt == null);

        builder.HasOne(x => x.TradeOffer)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.TradeOfferId)
            .HasConstraintName("fk_trade_offer_items_trade_offer_id");

        builder.HasOne(x => x.Sticker)
            .WithMany(x => x.TradeOfferItems)
            .HasForeignKey(x => x.StickerId)
            .HasConstraintName("fk_trade_offer_items_sticker_id");

        builder.HasIndex(x => x.TradeOfferId)
            .HasDatabaseName("ix_trade_offer_items_trade_offer_id");
    }
}
