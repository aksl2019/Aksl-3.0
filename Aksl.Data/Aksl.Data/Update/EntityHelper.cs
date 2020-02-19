using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

//https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.Core/src/ModelBinding/ModelBindingHelper.cs

namespace Aksl.Data
{
    public static class EntityHelper
    {
        public static Task TryUpdateEntryAsync<TEntry>(DbContext dbContext, TEntry entity, params Expression<Func<TEntry, object>>[] includeExpressions) where TEntry : class
        {
            dbContext.Entry(entity).State = EntityState.Unchanged;
            foreach (var property in includeExpressions)
            {
                //var propertyName = GetPropertyName(property);
                var propertyName = ExpressioHelperEx.GetExpressionText(property);
                dbContext.Entry(entity).Property(propertyName).IsModified = true;
            }

            return Task.CompletedTask;
        }
    }
}
