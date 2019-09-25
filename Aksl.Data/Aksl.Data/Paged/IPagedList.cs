using System;
using System.Collections.Generic;
using System.Text;

namespace Aksl.Data
{
    /// <summary>
    /// Paged list interface
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        int PageIndex { get; }

        int PageSize { get; }

        int TotalCount { get; }

        int TotalPages { get; }

     //   IEnumerable<T> Datas { get; }

        //bool HasPreviousPage { get; }

        //bool HasNextPage { get; }
    }
}
