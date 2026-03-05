using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Common.Models;

public sealed class PaginatedListWithCount<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IReadOnlyCollection<T> Items { get; }

    [JsonConstructor]
    public PaginatedListWithCount(
        int pageNumber,
        int pageSize,
        int totalPages,
        int totalCount,
        IReadOnlyCollection<T> items)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;
        TotalCount = totalCount;
        Items = items;
    }

    private PaginatedListWithCount(
        IReadOnlyCollection<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        Items = items;
    }

    public static async Task<PaginatedListWithCount<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);

        // Short-circuit: skip second query if no data
        if (count == 0)
            return new PaginatedListWithCount<T>([], 0, pageNumber, pageSize);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedListWithCount<T>(items, count, pageNumber, pageSize);
    }
}