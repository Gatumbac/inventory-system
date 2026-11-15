using Inventory.Core.Results;

namespace Inventory.Core.Tests;

public class PagedResultTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    public void TotalPages_RoundsUp(int totalCount, int pageSize, int expected)
    {
        var result = new PagedResult<int>()
        {
            Items = new List<int>(),
            Page = 1,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        Assert.Equal(expected, result.TotalPages);
    }
}
