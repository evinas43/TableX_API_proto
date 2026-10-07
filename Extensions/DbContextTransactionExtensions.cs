using System.Data;
using Microsoft.EntityFrameworkCore;

namespace TablexAPI.Extensions;

public static class DbContextTransactionExtensions
{
    public static Task<T> ExecuteInTransactionAsync<T>(
        this DbContext db,
        Func<CancellationToken, Task<T>> operation,
        IsolationLevel isolationLevel,
        CancellationToken ct = default)
    {
        var strategy = db.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();

            await using var transaction = await db.Database.BeginTransactionAsync(isolationLevel, ct);
            var result = await operation(ct);
            await transaction.CommitAsync(ct);
            return result;
        });
    }
}
