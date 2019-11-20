using System;

namespace Aksl.Retry
{
    public class Retry
    {
        #region Static Methods
        /// <summary>
        /// 再重新试
        /// </summary>
        public static Retry Yes { get; } = new Retry(true, TimeSpan.Zero);

        /// <summary>
        /// 设定重试时间
        /// </summary>
        /// <param name="retryAfter"></param>
        /// <returns></returns>
        public  static  Retry YesAfter(TimeSpan retryAfter) => new Retry(true, retryAfter);

        /// <summary>
        /// 不再试
        /// </summary>
        public  static Retry No { get; } = new Retry(false, TimeSpan.Zero);
        #endregion

        #region Constructors
        protected Retry(bool shouldBeRetried, TimeSpan retryAfter)
        {
            if (retryAfter != TimeSpan.Zero && retryAfter != retryAfter.Duration())
                throw new ArgumentOutOfRangeException(nameof(retryAfter));
            if (!shouldBeRetried && retryAfter != TimeSpan.Zero)
                throw new ArgumentException("Invalid combination. Should not be retried and retry after set");

            ShouldBeRetried = shouldBeRetried;
            RetryAfter = retryAfter;
        }
        #endregion

        #region Properties
        public bool ShouldBeRetried { get; }

        public TimeSpan RetryAfter { get; }
        #endregion
    }
}
