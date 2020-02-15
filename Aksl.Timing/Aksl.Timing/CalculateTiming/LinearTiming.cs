
namespace Aksl.Timing
{
    /// <summary>
    /// Linear Timing
    /// </summary>
    public class LinearTiming : CalculateTiming
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="minimumPeriodInSeconds">Minimum Period In Seconds</param>
        /// <param name="maximumPeriodInSeconds">Maximum Period In Seconds</param>
        public LinearTiming(int minimumPeriod = BaseTimes.MinimumTiming, int maximumPeriod = BaseTimes.MaximumTiming)
            : base(minimumPeriod, maximumPeriod)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Linear strategy
        /// </summary>
        /// <param name="attempts">attempts</param>
        /// <returns>timing</returns>
        public override double Get(ulong attempt)
        {
            //次数大于了最大值，就一直取最大值,否则按线性增加
            return attempt > 10 ? FrequencyInSeconds.Maximum : FrequencyInSeconds.Minimum + (((FrequencyInSeconds.Maximum - FrequencyInSeconds.Minimum) * .1) * attempt);
        }
        #endregion
    }
}