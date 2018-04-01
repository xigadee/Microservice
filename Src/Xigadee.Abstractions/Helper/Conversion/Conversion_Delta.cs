#region using
using System;
#endregion
namespace Xigadee
{

    public static partial class ConversionHelper
    {

        #region CalculateDelta(int now, int start)
        /// <summary>
        /// This method calculates the delta and takes in to account that the
        /// tick-count recycles to negative every 49 days.
        /// </summary>
        /// <param name="now">The current tick count.</param>
        /// <param name="start">The start point.</param>
        /// <returns>Returns the delta.</returns>
        public static int CalculateDelta(int now, int start)
        {
            int delta;
            if (now >= start)
                delta = now - start;
            else
            {
                //Do this, otherwise you'll be in a world of pain every 49 days.
                long upLimit = ((long)(int.MaxValue)) + Math.Abs(int.MinValue - now);
                delta = (int)(upLimit - start);
            }

            return delta;
        }
        #endregion

        /// <summary>
        /// This method calculates the tick count delta.
        /// </summary>
        /// <param name="tickStart">The start point.</param>
        /// <param name="tickNow">The current tick-count. If this is null Environment.TickCount is used.</param>
        /// <returns>The delta.</returns>
        public static int DeltaAsMs(int tickStart, int? tickNow = null)
        {
            return CalculateDelta(tickNow ?? Environment.TickCount, tickStart);
        }
        /// <summary>
        /// Returns the delta as a timespan.
        /// </summary>
        /// <param name="tickStart">The start point. If this is null, then the method returns null.</param>
        /// <param name="tickNow">The current tick-count. If this is null Environment.TickCount is used.</param>
        /// <returns>The delta as a timespan.</returns>
        public static TimeSpan? DeltaAsTimeSpan(int? tickStart, int? tickNow = null)
        {
            if (!tickStart.HasValue)
                return null;

            return TimeSpan.FromMilliseconds(DeltaAsMs(tickStart.Value, tickNow));
        }
        /// <summary>
        /// Returns the delta as a human readable string..
        /// </summary>
        /// <param name="tickStart">The start point.</param>
        /// <param name="tickNow">The current tick-count. If this is null Environment.TickCount is used.</param>
        /// <returns>A string containing the time span.</returns>
        public static string DeltaAsFriendlyTime(int tickStart, int? tickNow = null)
        {
            return ToFriendlyString(DeltaAsTimeSpan(tickStart, tickNow));
        }

        #region ToFriendlyString(TimeSpan? time, string defaultText="NA")

        static readonly Func<TimeSpan?, string> fnTimeConv = (time) =>
        {
            try
            {
                if (Math.Abs(time.Value.TotalMilliseconds) < 1000)
                    return string.Format("{0:F2}ms", time.Value.TotalMilliseconds);

                if (Math.Abs(time.Value.Days) > 0)
                    return time.Value.ToString(@"d'day'hh'h'mm'm'ss'.'ff's'");
                if (Math.Abs(time.Value.Hours) > 0)
                    return time.Value.ToString(@"hh'h'mm'm'ss'.'ff's'");
                if (Math.Abs(time.Value.Minutes) > 0)
                    return time.Value.ToString(@"mm'm'ss'.'ff's'");

                return time.Value.ToString(@"ss'.'ff's'");
            }
            catch (Exception)
            {
                return null;
            }
        };
        /// <summary>
        /// This helper converts a timespan in to a human readable time.
        /// </summary>
        /// <param name="timeIn">The TimeSpan object to convert.</param>
        /// <param name="nullName">The default text to display if the TimeSpan object is null. NA by default.</param>
        /// <returns>Returns a string representation of the time.</returns>
        public static string ToFriendlyString(this TimeSpan? timeIn, string nullName = "NA")
        {
            return timeIn.HasValue ? ToFriendlyString(timeIn.Value) : nullName;
        }
        /// <summary>
        /// This helper converts a timespan in to a human readable time.
        /// </summary>
        /// <param name="timeIn">The TimeSpan object to convert.</param>
        /// <returns>Returns a string representation of the time.</returns>
        public static string ToFriendlyString(this TimeSpan timeIn)
        {
            string output = fnTimeConv(timeIn);

            if (output == null)
                return "ERR";

            if (timeIn.TotalMilliseconds < 0)
                return "-" + output;

            return output;
        }
        #endregion

    }
}
