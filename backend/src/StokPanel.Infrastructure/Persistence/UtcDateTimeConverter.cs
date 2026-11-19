using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StokPanel.Infrastructure.Persistence
{
    internal sealed class UtcDateTimeConverter()
        : ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
}
