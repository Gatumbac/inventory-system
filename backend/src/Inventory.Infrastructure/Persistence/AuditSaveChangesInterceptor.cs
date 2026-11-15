using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Inventory.Infrastructure.Persistence;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampAuditDates(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StampAuditDates(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void StampAuditDates(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries().Where(HasAuditDates))
        {
            StampEntry(entry, now);
        }
    }

    private static bool HasAuditDates(EntityEntry entry)
    {
        return entry.Metadata.FindProperty("CreatedAt") != null
            && entry.Metadata.FindProperty("UpdatedAt") != null;
    }

    private static void StampEntry(EntityEntry entry, DateTime now)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Property("CreatedAt").CurrentValue = now;
            entry.Property("UpdatedAt").CurrentValue = now;
        }
        else if (entry.State == EntityState.Modified)
        {
            entry.Property("UpdatedAt").CurrentValue = now;
        }
    }
}
