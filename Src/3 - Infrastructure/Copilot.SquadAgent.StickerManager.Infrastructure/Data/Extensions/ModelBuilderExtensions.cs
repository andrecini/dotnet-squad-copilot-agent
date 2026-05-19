using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Extensions;

internal static class ModelBuilderExtensions
{
    internal static void ApplyUtcDateTimeConverters(this ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var utcNullableConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(utcConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(utcNullableConverter);
            }
        }
    }
}
