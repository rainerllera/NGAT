using System;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Domain.Base;

namespace NGAT.Business.Contracts.DataAccess
{
    public interface IUnitOfWork
    {
        /// <summary>
        ///     Commit all changes made in a container.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        ///     Gets the TEntity repository.
        /// </summary>
        /// <typeparam name="TEntity">Represents a database's entity.</typeparam>
        /// <returns>A <see cref="IRepository{TEntity}" /> object.</returns>
        IRepository<TEntity> Set<TEntity>() where TEntity : Entity;
    }
}
