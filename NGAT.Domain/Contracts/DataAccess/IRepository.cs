using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace NGAT.Business.Contracts.DataAccess
{
    /// <summary>
    /// Contract for the repository data access pattern
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : NGAT.Business.Domain.Base.Entity
    {
        /// <summary>
        /// The unit of work in this repository
        /// </summary>
        IUnitOfWork UnitOfWork { get; set; }

        /// <summary>
        /// Adds an entity to the repository
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Modifies an entity
        /// </summary>
        /// <param name="entity">The entity to edit</param>
        /// <returns>The edited entity</returns>
        TEntity Edit(TEntity entity);

        /// <summary>
        /// Finds an entity according to its key
        /// </summary>
        /// <param name="key">The key of the entity</param>
        /// <returns>The entity if it was found, null otherwise</returns>
        TEntity Find(params object[] key);

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns></returns>
        TEntity Delete(TEntity entity);

        /// <summary>
        /// Gets all entities in the repository
        /// </summary>
        /// <param name="filter">Filters the elements to retrieve</param>
        /// <param name="propertySelectors">Selects the (navigation) properties to include in the query</param>
        /// <returns>A list of selected elements</returns>
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] propertySelectors);
    }
}
