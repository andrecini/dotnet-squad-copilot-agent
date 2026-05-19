using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Token)
            .HasColumnName("token")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .HasColumnName("is_used")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
