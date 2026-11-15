using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public static class PagedQuery
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(IQueryable<T> query, PageRequest request)
    {
        var totalCount = await query.CountAsync();
        var skip = (request.Page - 1) * request.PageSize;
        var items = await query.Skip(skip).Take(request.PageSize).ToListAsync();

        var result = new PagedResult<T>()
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return result;
    }
}
