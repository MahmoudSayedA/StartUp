using System.Text.Json.Serialization;

namespace Application.Common.Models;

public class PaginatedList<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public IReadOnlyCollection<T> Items { get; }

    [JsonConstructor]
    public PaginatedList(
    int pageNumber,
    int pageSize,
    IReadOnlyCollection<T> items)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }


    public PaginatedList(IReadOnlyCollection<T> items, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, pageNumber, pageSize);
    }

}