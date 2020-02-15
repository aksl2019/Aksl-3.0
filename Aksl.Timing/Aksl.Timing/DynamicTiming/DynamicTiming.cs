using System;

namespace Aksl.Timing
{
    /// <summary>
    /// Dynamic Timing
    /// </summary>
    public abstract class DynamicTiming : IDynamicTiming
    {
        #region Members
        /// <summary>
        /// Attempts Made
        /// </summary>
        protected ulong _noWorkCount = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Mockable Constructor
        /// </summary>
        /// <param name="timing">Calculate Timing</param>
        public DynamicTiming(ICalculateTiming timing)
        {
            Timing = timing ?? throw new ArgumentNullException("timing");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Timing
        /// </summary>
        public virtual ICalculateTiming Timing { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Get Timing
        /// </summary>
        /// <param name="workWasDone">Work was done</param>
        /// <returns>New Timing</returns>
        public abstract double Get(bool workWasDone);
        #endregion
    }
}