using System;
using System.Collections.Generic;
using System.Text;

namespace Aksl.Data
{
    public class PageInfo
    {
        public PageInfo(int pageSize, int totalCount)
        {
            this.PageSize = pageSize;
            this.TotalCount = totalCount;

            TotalPages = this.TotalCount / this.PageSize;
            if (this.TotalCount % this.PageSize > 0)
            {
                TotalPages++;
            }
        }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public int TotalPages { get; private set; }
    }
}
