using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class StickerConfiguration : IEntityTypeConfiguration<Sticker>
{
    public void Configure(EntityTypeBuilder<Sticker> builder)
    {
        builder.ToTable("stickers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("uq_stickers_code");

        builder.Property(x => x.PlayerName)
            .HasColumnName("player_name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Rarity)
            .HasColumnName("rarity")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.TeamId)
            .HasColumnName("team_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(x => x.DeletedAt == null);

        builder.HasOne(x => x.Team)
            .WithMany(x => x.Stickers)
            .HasForeignKey(x => x.TeamId)
            .HasConstraintName("fk_stickers_team_id");

        builder.HasIndex(x => x.TeamId)
            .HasDatabaseName("ix_stickers_team_id");
    }
}
