using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.Linq.Dynamic
{
    public static class EntityExtensions
    {
        #region ContainsExpression
        /// <summary>
        /// Builds a Contains expression
        /// </summary>
        /// <example>var query2 = context.Entities.Where(ContainsExpression<Entity, int>(e => e.Id, ids));</example>
        public static Expression<Func<TElement, bool>> ContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException( nameof(valueSelector)); }
            if (null == values) { throw new ArgumentNullException(nameof(values)); }

            ParameterExpression p = valueSelector.Parameters.Single();
            // p => valueSelector(p) == values[0] || valueSelector(p) == ...
            if (!values.Any())
            {
                return e => false;
            }

            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
        #endregion

        #region WhereIn
        private static Expression<Func<TElement, bool>> GetWhereInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector,IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
            {
                return e => false;
            }

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        /// <summary> 
        /// Return the element that the specified property's value is contained in the specifiec values 
        /// </summary> 
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns> 
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source,Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
        {
            return source.Where(GetWhereInExpression(propertySelector, values));
        }

        /// <summary> 
        /// Return the element that the specified property's value is contained in the specifiec values 
        /// </summary> 
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns> 
        //var query2 = context.Entities.WhereIn<Entity, int>(e => e.Id, ids))
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source,Expression<Func<TElement, TValue>> propertySelector,IEnumerable<TValue> values)
        {
            return source.Where(GetWhereInExpression(propertySelector, values));
        }
        #endregion

        #region WhereNotIn
        private static Expression<Func<TElement, bool>> GetWhereNotInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
            {
                return e => true;
            }

            var unequals = values.Select(value =>(Expression)Expression.NotEqual(propertySelector.Body,Expression.Constant(value, typeof(TValue))));
            var body = unequals.Aggregate((accumulate, unequal) => Expression.And(accumulate, unequal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(this IQueryable<TElement> source,Expression<Func<TElement, TValue>> propertySelector,params TValue[] values)
        {
            return source.Where(GetWhereNotInExpression(propertySelector, values));
        }

        //var query2 = context.Entities.WhereNotIn<Entity, int>(e => e.Id, ids))
        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(this IQueryable<TElement> source,Expression<Func<TElement, TValue>> propertySelector,IEnumerable<TValue> values)
        {
            return source.Where(GetWhereNotInExpression(propertySelector, values));
        }
        #endregion
    }
}
