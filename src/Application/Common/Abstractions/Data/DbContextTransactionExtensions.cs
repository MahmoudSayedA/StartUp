using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Abstractions.Data;

public static class DbContextTransactionExtensions
{
    public static async Task CommitAsync(this IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        await transaction.CommitAsync(cancellationToken);
    }

    public static async Task RollbackAsync(this IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        await transaction.RollbackAsync(cancellationToken);
    }
}
