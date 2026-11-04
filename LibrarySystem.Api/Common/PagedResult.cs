using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace LibrarySystem.Api.Common
{
    //pagination
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; private set; }
        public int PageSize { get; private set; }
        public int PageNumber { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        private PagedResult(IReadOnlyList<T> items,int pagesize,int pagenumber,int totalcount)
        {
            Items = items;
            PageNumber = pagenumber;
            PageSize = pagesize;
            TotalCount= totalcount;
        }

        public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int pagenumber, int pagesize, CancellationToken cancellationToken = default)
        {
            pagesize = Math.Clamp(pagesize,1, 100);
            pagenumber = Math.Max(1, pagenumber);

            var totalcount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pagenumber - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync(cancellationToken);

            return new PagedResult<T>
            (
               items,
               pagesize,
               pagenumber,
               totalcount

            );

        }
    }
}
