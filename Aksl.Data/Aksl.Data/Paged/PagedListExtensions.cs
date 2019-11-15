using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Aksl.Data
{
    public static class PagedListExtensions
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T">实体数据</typeparam>
        /// <param name="source">数据集合</param>
        /// <param name="pageIndex">当前页码, 0开始</param>
        /// <param name="pageSize">每页大下</param>
        /// <returns></returns>
        public static async ValueTask<IPagedList<T>> AddPagedAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        {
            var totalCount = source.Count();

            var pagedList = await source
                                 .Skip(pageSize * pageIndex)
                                 .Take(pageSize)
                                 .ToListAsync();

            return new PagedList<T>(pageIndex, pageSize, totalCount, pagedList);
        }

        //public static async IAsyncEnumerable<T> AddPagedAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        //{
        //    var totalCount = source.Count();

        //    var pagedList = await source
        //                           .Skip(pageSize * pageIndex)
        //                           .Take(pageSize)
        //                           .AsAsyncEnumerable();

        //    return new PagedList<T>(pageIndex, pageSize, totalCount, pagedList);
        //}

        public static ValueTask<IPagedList<T>> AddPagedAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize) where T : class
        {
            var totalCount = source.Count();

            var pagedList = source
                            .Skip(pageSize * pageIndex)
                            .Take(pageSize)
                            .ToList();

            return new ValueTask<IPagedList<T>>(new PagedList<T>(pageIndex, pageSize, totalCount, pagedList));
        }
    }
}
