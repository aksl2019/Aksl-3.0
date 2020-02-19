using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Aksl.Data
{
    public class ExpressioHelperEx
    {
        /// <summary>
        /// Gets the model name from a lambda expression.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>

        public static string GetExpressionText<TModel>(Expression<Func<TModel, object>> expression)
        {
            var expr = (LambdaExpression)expression;

            if (expr.Body.NodeType == ExpressionType.Convert)
            {
                var ue = expression.Body as UnaryExpression;

                var properties = GetProperties(ue.Operand).ToList();
                var propertyNames = properties.Select(p => p.Name).ToList();

                string propertyName= string.Join(".", propertyNames);
                return propertyName;

                // return string.Join(".", GetProperties(ue.Operand).Select(p => p.Name));
            }

            return Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.ExpressionHelper.GetExpressionText(expr);
        }


        /// <summary>
        /// Return a list of properties for an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A list of properties.</returns>
        private static IEnumerable<PropertyInfo> GetProperties(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null) yield break;

            var property = memberExpression.Member as PropertyInfo;

            foreach (var propertyInfo in GetProperties(memberExpression.Expression))
            {
                yield return propertyInfo;

            }
            yield return property;
        }
    }
}
