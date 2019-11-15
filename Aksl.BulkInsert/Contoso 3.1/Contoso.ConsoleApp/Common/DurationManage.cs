using System;
using System.Text;

namespace Contoso.ConsoleApp
{
    public class DurationManage
    {
        public TimeSpan TotalTime { get; set; } = TimeSpan.Zero;

        public TimeSpan MaxTime { get; set; } = TimeSpan.Zero;

        public TimeSpan AvgTime => OnAvg();

        private TimeSpan OnAvg()
        {
            var avg = Math.Round((TotalTime.TotalMilliseconds / Count), 4);
            return TimeSpan.FromMilliseconds(avg);
        }

        public int Count { get; set; } = 1;

        public int TotalCount { get; set; } = 0;

        public void Reset()
        {
            TotalTime = TimeSpan.Zero;
            MaxTime = TimeSpan.Zero;
            TotalCount=0;
            Count = 1;
        }

        public string TotalTimeToString => $"total duration:\"{ TotalTime} \" avg:\"{AvgTime}\"";

        public string TotalToMSString => $"total duration: \"{ TotalTime.TotalMilliseconds} ms\" avg:\"{ AvgTime.TotalMilliseconds} ms\"";

        public string AvgTimeToString => $"avg:\"{AvgTime}\"";

        public string AvgToMSString => $"avg:\"{AvgTime.TotalMilliseconds} ms\"";

        public string MaxAndAvgToString => $"max:\"{MaxTime} \" avg:\"{AvgTime}\"";

        public string MaxTimeToString => $"max duration:\"{MaxTime}\"";

        public string MaxToMSString => $"max duration:\"{ MaxTime.TotalMilliseconds} ms\"";

        public string ParallelString()
        {
            StringBuilder strbuf = new StringBuilder($"duration: ");
            if (AvgTime > TimeSpan.Zero)
            {
                strbuf.Append(AvgTimeToString);
            }
            if (MaxTime > TimeSpan.Zero)
            {
                strbuf.Append(MaxTimeToString);
            }
            return strbuf.ToString();
        }

        public string ParallelMSString()
        {
            StringBuilder strbuf = new StringBuilder($"duration: ");
            if (AvgTime > TimeSpan.Zero)
            {
                strbuf.Append(AvgToMSString);
            }
            strbuf.Append(" ");
            if (MaxTime > TimeSpan.Zero)
            {
                strbuf.Append(MaxToMSString);
            }
            return strbuf.ToString();
        }

        public override string ToString()
        {
            StringBuilder strbuf = new StringBuilder($"duration: ");
            if (TotalTime > TimeSpan.Zero)
            {
                strbuf.Append($"total: \"{ TotalTime}\" ");
                strbuf.Append($"avg: \"{ AvgTime}\" ");
            }
            if (MaxTime > TimeSpan.Zero)
            {
                strbuf.Append($"max: \"{MaxTime}\" ");
            }
            return strbuf.ToString();
        }

        public string ToMSString()
        {
            StringBuilder strbuf = new StringBuilder($"duration: ");
            if (TotalTime > TimeSpan.Zero)
            {
                strbuf.Append($"total: \"{ TotalTime.TotalMilliseconds} ms\" ");
                strbuf.Append($"avg: \"{ AvgTime} ms\" ");
            }
            if (MaxTime > TimeSpan.Zero)
            {
                strbuf.Append($"max: \"{MaxTime.TotalMilliseconds} ms\" ");
            }
            return strbuf.ToString();
        }

        public  TimeSpan GetMaxTimeValue( TimeSpan source, TimeSpan other)
        {
            return source.Ticks < other.Ticks ? other : source;
        }
    }
}
