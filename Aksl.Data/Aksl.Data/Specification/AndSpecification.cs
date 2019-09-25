using System;
using System.Linq.Expressions;

namespace Aksl.Data
{
    /// <summary>
    /// A logic AND Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class AndSpecification<T> : CompositeSpecification<T> where T : class
    {
        #region Members
        private readonly ISpecification<T> _leftSideSpecification = null;
        private ISpecification<T> _rightSideSpecification = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for AndSpecification
        /// </summary>
        /// <param name="leftSide">Left side specification</param>
        /// <param name="rightSide">Right side specification</param>
        public AndSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
        {
            _leftSideSpecification = leftSide ?? throw new ArgumentNullException(nameof(leftSide));
            _rightSideSpecification = rightSide ?? throw new ArgumentNullException(nameof(rightSide));
        }
        #endregion

        #region  Overrides
        /// <summary>
        /// Left side specification
        /// </summary>
        public override ISpecification<T> LeftSideSpecification => _leftSideSpecification;

        /// <summary>
        /// Right side specification
        /// </summary>
        public override ISpecification<T> RightSideSpecification => _rightSideSpecification;

        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            Expression<Func<T, bool>> left = _leftSideSpecification.SatisfiedBy();
            Expression<Func<T, bool>> right = _rightSideSpecification.SatisfiedBy();

            return (left.And(right));
        }
        #endregion
    }
}
