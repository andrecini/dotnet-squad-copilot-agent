using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class UserCollectionConfiguration : IEntityTypeConfiguration<UserCollection>
{
    public void Configure(EntityTypeBuilder<UserCollection> builder)
    {
        builder.ToTable("user_collections");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.StickerId)
            .HasColumnName("sticker_id")
            .IsRequired();

        builder.Property(x => x.QuantityOwned)
            .HasColumnName("quantity_owned")
            .IsRequired();

        builder.Property(x => x.QuantityDuplicate)
            .HasColumnName("quantity_duplicate")
            .IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(x => x.DeletedAt == null);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserCollections)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("fk_user_collections_user_id");

        builder.HasOne(x => x.Sticker)
            .WithMany(x => x.UserCollections)
            .HasForeignKey(x => x.StickerId)
            .HasConstraintName("fk_user_collections_sticker_id");

        builder.HasIndex(x => new { x.UserId, x.StickerId })
            .IsUnique()
            .HasDatabaseName("ix_user_collections_user_id_sticker_id");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_user_collections_user_id");

        builder.HasIndex(x => x.StickerId)
            .HasDatabaseName("ix_user_collections_sticker_id");
    }
}
