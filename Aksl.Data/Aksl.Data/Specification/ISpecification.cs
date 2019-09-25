using System;
using System.Linq.Expressions;

namespace Aksl.Data
{
    /// <summary>
    /// Base contract for Specification pattern, for more information
    /// about this pattern see http://martinfowler.com/apsupp/spec.pdf
    /// or http://en.wikipedia.org/wiki/Specification_pattern.
    /// This is really a variant implementation where we have added Linq and
    /// lambda expression into this pattern.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public interface ISpecification<TEntity> where TEntity : class
    {
        /// <summary>
        /// Check if this specification is satisfied by a 
        /// specific expression lambda
        /// </summary>
        /// <returns></returns>
        Expression<Func<TEntity, bool>> SatisfiedBy();

        ///// <summary>
        ///// Returns a <see cref="bool"/> value which indicates whether the specification
        ///// is satisfied by the given object.
        ///// </summary>
        ///// <param name="obj">The object to which the specification is applied.</param>
        ///// <returns>True if the specification is satisfied, otherwise false.</returns>
        //bool IsSatisfiedBy(TEntity entity);

        ///// <summary>
        ///// Gets the LINQ expression which represents the current specification.
        ///// </summary>
        ///// <returns>The LINQ expression.</returns>
        //Expression<Func<TEntity, bool>> ToExpression();
    }
}
