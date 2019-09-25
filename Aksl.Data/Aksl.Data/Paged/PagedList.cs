using System;
using System.Collections.Generic;

namespace Aksl.Data
{
    //PagedResult
    [Serializable]
    public sealed class PagedList<T> : List<T>,IPagedList<T> where T : class 
    {
        public PagedList(int pageIndex, int pageSize, int totalCount, IEnumerable<T> pagedList)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;

            TotalPages = this.TotalCount / this.PageSize;
            if (this.TotalCount % this.PageSize > 0)
            {
                TotalPages++;
            }

            this.AddRange(pagedList);
        }

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public int TotalPages { get; private set; }
    }


    public sealed class QueryDescription
    {
        public long? MaxResults { get; set; }
    }
}
