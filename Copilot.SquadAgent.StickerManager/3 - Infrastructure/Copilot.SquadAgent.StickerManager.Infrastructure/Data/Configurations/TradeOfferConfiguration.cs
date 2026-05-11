using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class TradeOfferConfiguration : IEntityTypeConfiguration<TradeOffer>
{
    public void Configure(EntityTypeBuilder<TradeOffer> builder)
    {
        builder.ToTable("trade_offers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(x => x.DeletedAt == null);

        builder.HasOne(x => x.User)
            .WithMany(x => x.TradeOffers)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("fk_trade_offers_user_id");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_trade_offers_user_id");
    }
}
