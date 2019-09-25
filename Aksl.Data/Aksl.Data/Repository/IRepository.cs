using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Aksl.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IRepository<TEntity> where TEntity : class
    {
        #region Properties
        DbSet<TEntity> Entities { get; }

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<TEntity> TableNoTracking { get; }

        #endregion

        #region Get Methods
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        // TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        //Task<TEntity> InsertAsync(TEntity entity);
        Task InsertAsync(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
       // Task InsertAsync(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> BulkInsertAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Update Methods
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        Task UpdateAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        Task DeleteAsync(IEnumerable<TEntity> entities);
        #endregion
    }
}
