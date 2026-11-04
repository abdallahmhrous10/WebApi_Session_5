using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LibrarySystem.Api.Data.Interceptors
{
    public class LoggingSaveChangesInterceptor(ILogger<LoggingSaveChangesInterceptor> _logger) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync
            (DbContextEventData eventData, InterceptionResult<int> result,CancellationToken cancellationToken = default)
        {
            var Entries = eventData.Context?.ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                .Select(entry => $"{entry.Metadata.ClrType.Name} : {entry.State}").ToArray() ?? Array.Empty<string>();

                    _logger.LogInformation(
               "EF Core is saving {Count} entity changes: {Changes}",
               Entries.Length,
               string.Join(", ", Entries));

            return base.SavingChangesAsync(
              eventData,
              result,
              cancellationToken);
        }

        public override ValueTask<int> SavedChangesAsync
            (SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
         "EF Core saved {Count} state entries successfully.",
         result);

            return base.SavedChangesAsync(
             eventData,
             result,
             cancellationToken);
        }

        public override Task SaveChangesFailedAsync
         (DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            _logger.LogError(
            eventData.Exception,
            "EF Core failed while saving changes.");

            return base.SaveChangesFailedAsync(
             eventData,
             cancellationToken);
        }
    }
}
