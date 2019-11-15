using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Aksl.Data
{
    public static class IQueryableExtensions
    {

        #region Include Methods
        public static IQueryable<TEntity> Includes<TEntity>(this IQueryable<TEntity> queryable, params Expression<Func<TEntity, object>>[] includeExpressions) where TEntity : class
        {
            var query = queryable;
            query = includeExpressions.Aggregate(query, (current, includeExp) => current.Include(includeExp));
            return query;
        }

        public static IQueryable<TEntity> IncludeStrings<TEntity>(this IQueryable<TEntity> queryable, params string[] includeStrings) where TEntity : class
        {
            var query = includeStrings.Aggregate(queryable, (current, includeString) => current.Include(includeString));
            return query;
            //return Include<TEntity>(queryable, AnalyzeExpressionPath(path));
        }
        #endregion

        #region Order Methods
        public static IQueryable<TEntity> ByOrders<TEntity>(this IQueryable<TEntity> queryable, params Expression<Func<TEntity, object>>[] orderByExpressions) where TEntity : class
        {
            var query = orderByExpressions.Aggregate(queryable, (current, orderByExp) => current.OrderBy(orderByExp));
            return query;
        }

        public static IQueryable<TEntity> ByOrderDescendings<TEntity>(this IQueryable<TEntity> queryable, params Expression<Func<TEntity, object>>[] orderByDescendingExpressions) where TEntity : class
        {
            var query = orderByDescendingExpressions.Aggregate(queryable, (current, orderByExp) => current.OrderByDescending(orderByExp));
            return query;
        }
        #endregion

        #region Spec Methods
        public static IQueryable<TEntity> BySpec<TEntity>(this IQueryable<TEntity> queryable, ISpecification<TEntity> specification) where TEntity : class
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            var criteria = specification.SatisfiedBy();
            var query = queryable.Where(criteria);
            return query;
        }

        //public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, bool>> filter) where TEntity : class
        //{
        //    if (filter == null)
        //    {
        //        throw new ArgumentNullException(nameof(filter));
        //    }

        //    return queryable.Where(filter);

        //}
        #endregion

        #region Filter Methods
        public static IQueryable<TEntity> Filters<TEntity>(this IQueryable<TEntity> queryable, params Expression<Func<TEntity, bool>>[] filters) where TEntity : class
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var query = filters.Aggregate(queryable, (current, filter) => current.Where(filter));
            return query;
        }
        #endregion

        #region Count Methods
        //public static async ValueTask<int> CountAsync<TEntity>(this IQueryable<TEntity> queryable) where TEntity : class
        //{
        //    var totalCount = await queryable.CountAsync();
        //    return totalCount;
        //}
        #endregion

        #region Page Methods
        public static IQueryable<TEntity> PageBy<TEntity>(this IQueryable<TEntity> queryable, int pageIndex = 0, int pageSize = int.MaxValue) where TEntity : class
        {
            if (pageIndex < 0)
            {
                throw new ArgumentNullException(nameof(pageIndex));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentNullException(nameof(pageSize));
            }

            var pagedQuery = queryable.Skip(pageSize * pageIndex)
                                      .Take(pageSize);
            return pagedQuery;
        }

        public static async ValueTask<(IQueryable<TEntity> PagedQuery, int TotalCount)> PageByAsync<TEntity>(this IQueryable<TEntity> queryable, int pageIndex = 0, int pageSize = int.MaxValue) where TEntity : class
        {
            if (pageIndex < 0)
            {
                throw new ArgumentNullException(nameof(pageIndex));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentNullException(nameof(pageSize));
            }

            var totalCount = await queryable.CountAsync();
            var pagedQuery = queryable.PageBy<TEntity>(pageIndex, pageSize);

            return (pagedQuery, totalCount);
        }

        public static IQueryable<TEntity> PageBySpec<TEntity>(this IQueryable<TEntity> queryable, ISpecification<TEntity> specification, int pageIndex = 0, int pageSize = int.MaxValue) where TEntity : class
        {
            var pagedQuery = queryable.BySpec(specification)
                                      .PageBy(pageIndex, pageSize);

            return pagedQuery;
        }

        public static async ValueTask<(IQueryable<TEntity> PagedQuery, int TotalCount)> PageBySpecAsync<TEntity>(this IQueryable<TEntity> queryable, ISpecification<TEntity> specification, int pageIndex = 0, int pageSize = int.MaxValue) where TEntity : class
        {
            var totalCount = await queryable.CountAsync();
            var pagedQuery = queryable.PageBySpec(specification, pageIndex, pageSize);

            return (pagedQuery, totalCount);
        }

        public static IQueryable<TEntity> PageByFilters<TEntity>(this IQueryable<TEntity> queryable, int pageIndex = 0, int pageSize = int.MaxValue, params Expression<Func<TEntity, bool>>[] filters) where TEntity : class
        {
            IQueryable<TEntity> pagedQuery = queryable.Filters(filters).PageBy(pageIndex, pageSize);

            return pagedQuery;
        }

        public static async ValueTask<(IQueryable<TEntity> PagedQuery, int TotalCount)> PagingByFiltersAsync<TEntity>(this IQueryable<TEntity> queryable, int pageIndex = 0, int pageSize = int.MaxValue, params Expression<Func<TEntity, bool>>[] filters) where TEntity : class
        {
            var totalCount = await queryable.CountAsync();
            var pagedQuery = queryable.PageByFilters(pageIndex, pageSize, filters);

            return (pagedQuery, totalCount);
        }

        //public static IQueryable<TEntity> Paginate<TEntity, S>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, S>> orderBy, int pageIndex, int pageCount, bool ascending) where TEntity : class
        //{
        //    return queryable.OrderBy(orderBy).Skip((pageIndex * pageCount)).Take(pageCount);
        //}
        #endregion

        #region Private Methods
        private static string AnalyzeExpressionPath<TEntity, S>(Expression<Func<TEntity, S>> expression) where TEntity : class
        {
            if (expression == null)
            {
                throw new ArgumentNullException("Expression Path Not Valid");
            }

            MemberExpression body = expression.Body as MemberExpression;
            if (
                    (
                    (body == null)
                    ||
                    !body.Member.DeclaringType.IsAssignableFrom(typeof(TEntity))
                    )
                    ||
                    (body.Expression.NodeType != ExpressionType.Parameter)
                 )
            {
                throw new ArgumentException("Expression Path Not Valid");
            }
            else
            {
                return body.Member.Name;
            }
        }
        #endregion
    }
}
