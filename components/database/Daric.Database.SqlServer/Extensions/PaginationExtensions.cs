using Daric.Database.Abstraction.Models.Pagination;

using Microsoft.EntityFrameworkCore;

namespace Daric.Database.SqlServer.Extensions;

public static class PageResultExtensions
{
    public static PagesResultModel<T> ToPageResult<T>(this IQueryable<T> query, int pageIndex, int pageSize)
    {
        var count = query.Count();
        if (pageSize > 0)
        {
            query = query.Skip((pageIndex) * pageSize).Take(pageSize);
        }
        return new PagesResultModel<T> { Count = count, Data = [.. query] };
    }

    public static async Task<PagesResultModel<T>> ToPageResultAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize)
    {
        var count = await query.CountAsync();
        if (pageSize > 0)
        {
            query = query.Skip((pageIndex) * pageSize).Take(pageSize);
        }
        var data = await query.ToListAsync();
        return new PagesResultModel<T> { Count = count, Data = data };
    }

    public static List<T> ToList<T>(this PagesResultModel<T> page) => page.Data;

}
