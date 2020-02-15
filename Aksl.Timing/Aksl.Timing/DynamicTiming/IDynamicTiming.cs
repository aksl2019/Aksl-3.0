using System;

namespace Aksl.Timing
{
    #region IDynamicTiming
    /// <summary>
    /// Dynamic Timing
    /// </summary>
    public interface IDynamicTiming
    {
        #region Properties
        /// <summary>
        /// Timing Helper
        /// </summary>
        ICalculateTiming Timing { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Get Timing
        /// </summary>
        /// <param name="workWasDone">Work Was Done</param>
        /// <returns>Timing</returns>
        double Get(bool workWasDone);
        #endregion
    }
    #endregion
}