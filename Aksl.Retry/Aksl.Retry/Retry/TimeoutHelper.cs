// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Aksl.Retry
{
    internal struct TimeoutHelper
    {
        // public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(int.MaxValue);
        private TimeSpan _originalTimeout;
        private DateTime _deadline;
        private bool _deadlineSet;

        public TimeoutHelper(TimeSpan timeout)
            : this(timeout, false)
        {
        }

        public TimeoutHelper(TimeSpan timeout, bool startTimeout)
        {
            _originalTimeout = timeout;
            _deadline = DateTime.MaxValue;
            _deadlineSet = (timeout == TimeSpan.MaxValue);

            if (startTimeout && !_deadlineSet)
            {
                this.SetDeadline();
            }
        }

        public TimeSpan OriginalTimeout => _originalTimeout;

        //public static bool IsTooLarge(TimeSpan timeout)
        //{
        //    return (timeout > TimeoutHelper.MaxWait) && (timeout != TimeSpan.MaxValue);
        //}

        //public static TimeSpan FromMilliseconds(int milliseconds)
        //{
        //    if (milliseconds == Timeout.Infinite)
        //    {
        //        return TimeSpan.MaxValue;
        //    }

        //    return TimeSpan.FromMilliseconds(milliseconds);
        //}

        //public static int ToMilliseconds(TimeSpan timeout)
        //{
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        return Timeout.Infinite;
        //    }

        //    long ticks = timeout.Ticks;
        //    if (ticks / TimeSpan.TicksPerMillisecond > int.MaxValue)
        //    {
        //        return int.MaxValue;
        //    }

        //    return checked((int)(ticks / TimeSpan.TicksPerMillisecond));
        //}

        //public static TimeSpan Min(TimeSpan val1, TimeSpan val2)
        //{
        //    if (val1 > val2)
        //    {
        //        return val2;
        //    }

        //    return val1;
        //}

        //public static DateTime Min(DateTime val1, DateTime val2)
        //{
        //    if (val1 > val2)
        //    {
        //        return val2;
        //    }

        //    return val1;
        //}

        //public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2)
        //{
        //    return new TimeSpan(Add(timeout1.Ticks, timeout2.Ticks));
        //}

        //private static long Add(long firstTicks, long secondTicks)
        //{
        //    if (firstTicks == long.MaxValue || firstTicks == long.MinValue)
        //    {
        //        return firstTicks;
        //    }

        //    if (secondTicks == long.MaxValue || secondTicks == long.MinValue)
        //    {
        //        return secondTicks;
        //    }

        //    if (firstTicks >= 0 && long.MaxValue - firstTicks <= secondTicks)
        //    {
        //        return long.MaxValue - 1;
        //    }

        //    if (firstTicks <= 0 && long.MinValue - firstTicks >= secondTicks)
        //    {
        //        return long.MinValue + 1;
        //    }

        //    return checked(firstTicks + secondTicks);
        //}

        //public static DateTime Add(DateTime time, TimeSpan timeout)
        //{
        //    if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
        //    {
        //        return DateTime.MaxValue;
        //    }
        //    if (timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout)
        //    {
        //        return DateTime.MinValue;
        //    }
        //    return time + timeout;
        //}

        //public static DateTime Subtract(DateTime time, TimeSpan timeout)
        //{
        //    return Add(time, TimeSpan.Zero - timeout);
        //}

        //public static TimeSpan Divide(TimeSpan timeout, int factor)
        //{
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        return TimeSpan.MaxValue;
        //    }
        //    return new TimeSpan((timeout.Ticks / factor) + 1);
        //}

        //public static void ThrowIfNegativeArgument(TimeSpan timeout)
        //{
        //    ThrowIfNegativeArgument(timeout, nameof(timeout));
        //}

        //public static void ThrowIfNegativeArgument(TimeSpan timeout, string argumentName)
        //{
        //    if (timeout < TimeSpan.Zero)
        //    {
        //        throw new ArgumentOutOfRangeException(argumentName, timeout, string.Format(CultureInfo.CurrentCulture, "Argument {0} must be a non-negative timeout value. The provided value was {1}.", argumentName, timeout));
        //    }
        //}

        //public static void ThrowIfNonPositiveArgument(TimeSpan timeout)
        //{
        //    ThrowIfNonPositiveArgument(timeout, nameof(timeout));
        //}

        //public static void ThrowIfNonPositiveArgument(TimeSpan timeout, string argumentName)
        //{
        //    if (timeout <= TimeSpan.Zero)
        //    {
        //        throw new ArgumentOutOfRangeException(argumentName, timeout, string.Format(CultureInfo.CurrentCulture,"Argument {0} must be a positive timeout value. The provided value was {1}.", argumentName, timeout));
        //    }
        //}

        //public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
        //{
        //    ThrowIfNegativeArgument(timeout);
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        waitHandle.WaitOne();
        //        return true;
        //    }

        //    return waitHandle.WaitOne(timeout);
        //}

        public TimeSpan RemainingTime()
        {
            if (!_deadlineSet)
            {
                this.SetDeadline();
                return _originalTimeout;
            }

            if (_deadline == DateTime.MaxValue)
            {
                return TimeSpan.MaxValue;
            }

            TimeSpan remaining = _deadline - DateTime.UtcNow;
            if (remaining <= TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }

            return remaining;
        }

        public TimeSpan ElapsedTime() => _originalTimeout - RemainingTime();

        public void SetDeadline()
        {
            _deadline = DateTime.UtcNow + _originalTimeout;
            _deadlineSet = true;
        }
    }
}