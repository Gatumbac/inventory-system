namespace Inventory.Core.Results;

public class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; set; }
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector)
    {
        var result = new PagedResult<TOut>()
        {
            Items = Items.Select(selector).ToList(),
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount
        };

        return result;
    }
}
