using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dragon.Common.Objects
{
    [DebuggerDisplay("Range = {StartInclusiveDate.ToShortDateString()} - {EndInclusiveDate.ToShortDateString()}")]
    public class DateRange
    {
        private System.DateTime? m_startInclusiveDate;
        private System.DateTime? m_endInclusiveDate;

        #region Properties

        public System.DateTime StartInclusiveDate
        {
            get { return m_startInclusiveDate ?? System.DateTime.MinValue; }
        }

        public System.DateTime EndInclusiveDate
        {
            get { return m_endInclusiveDate ?? System.DateTime.MaxValue; }
        }

        public bool HasStart
        {
            get { return m_startInclusiveDate.HasValue; }
        }

        public bool HasEnd
        {
            get { return m_endInclusiveDate.HasValue; }
        }

        #endregion

        #region Constructors

        public DateRange()
        {
        }

        public DateRange(System.DateTime startInclusive)
        {
            m_startInclusiveDate = startInclusive;
        }



        public DateRange(System.DateTime startInclusive, System.DateTime endInclusive)
        {
            if (startInclusive > endInclusive)
            {
                throw new ArgumentException("End is before start.");
            }

            m_startInclusiveDate = startInclusive;
            m_endInclusiveDate = endInclusive;
        }

        #endregion

        #region Public Methods

        public bool Contains(System.DateTime d)
        {
            return (StartInclusiveDate <= d && EndInclusiveDate >= d);
        }

        public bool ContainsIgnoreTime(System.DateTime d)
        {
            return (m_startInclusiveDate.Value.Date <= d.Date && m_endInclusiveDate.Value.Date >= d.Date);
        }

        public static DateRange Create(System.DateTime startInclusive, System.DateTime endExclusive)
        {
            return new DateRange(startInclusive, endExclusive.AddTicks(-1));
        }

        // 
        /// <summary>
        /// Gets the mathematical intersection of 2 date ranges
        /// </summary>
        /// <param name="dr">The second date range used for interception</param>
        /// <returns>A DateRange which contains the intersected range, null if 2 Ranges do not intercept</returns>
        public DateRange IntersectWith(DateRange dr)
        {
            var startDate = StartInclusiveDate < dr.StartInclusiveDate ? dr.StartInclusiveDate : StartInclusiveDate;
            var endDate = EndInclusiveDate < dr.EndInclusiveDate ? EndInclusiveDate : dr.EndInclusiveDate;
            if (startDate > endDate)
                return null;

            return new DateRange(startDate, endDate);
        }

        public bool IntersectsWith(DateRange dr)
        {
            return IntersectsWith(dr.StartInclusiveDate, dr.EndInclusiveDate);
        }

        public bool IntersectsWith(System.DateTime startInclusiveDate, System.DateTime endInclusiveDate)
        {
            // > ------<---->----------
            // a --------[-]-----------
            // b --[-----]-------------
            // c ---------[-----]------
            // d --[---------------]---

            var myStartAfterTheirStart = m_startInclusiveDate >= startInclusiveDate;
            var myStartBeforeTheirStart = m_startInclusiveDate <= startInclusiveDate;

            var myEndAfterTheirEnd = m_endInclusiveDate >= endInclusiveDate;
            var myEndBeforeTheirEnd = m_endInclusiveDate <= endInclusiveDate;

            var myStartBeforeTheirEnd = m_startInclusiveDate <= endInclusiveDate;
            var myEndAfterTheirStart = m_endInclusiveDate >= startInclusiveDate;

            var scenarioA = myStartBeforeTheirStart && myEndAfterTheirEnd;
            var scenarioB = myStartAfterTheirStart && myStartBeforeTheirEnd;
            var scenarioC = myEndAfterTheirStart && myEndBeforeTheirEnd;
            var scenarioD = myStartAfterTheirStart && myEndBeforeTheirEnd;

            return (scenarioA || scenarioB || scenarioC || scenarioD);
        }

        public void EndOn(System.DateTime dt)
        {
            if (dt < m_startInclusiveDate) throw new ArgumentException("End is before start.");
            m_endInclusiveDate = dt;
        }

        public bool IsNow(System.DateTime nowReference)
        {
            return m_startInclusiveDate <= nowReference && m_endInclusiveDate >= nowReference;
        }

        public bool IsNow()
        {
            return IsNow(System.DateTime.Now);
        }

        public int DaysWithinRange()
        {
            return (EndInclusiveDate - StartInclusiveDate).Days;
        }

        public string ToShortRangeString()
        {
            return StartInclusiveDate.ToShortDateString() + " - " + EndInclusiveDate.ToShortDateString();
        }

        #endregion

        #region Fluent

        public static DateRange WithExclusiveEnd(System.DateTime startInclusive, System.DateTime endExclusive)
        {
            return new DateRange(startInclusive, endExclusive.AddTicks(-1));
        }

        public static DateRange StartingOn(System.DateTime startInclusive)
        {
            var dr = new DateRange(startInclusive);
            return dr;
        }

        public static DateRange For(System.DateTime startInclusive, System.DateTime endInclusive)
        {
            if (endInclusive < startInclusive) throw new ArgumentException("End is before start.");
            var dr = new DateRange();
            dr.m_startInclusiveDate = startInclusive;
            dr.m_endInclusiveDate = endInclusive;
            return dr;
        }

        public static DateRange Empty()
        {
            return new DateRange();
        }

        #endregion

        #region Misc

        public static bool ContainsToday(System.DateTime startInclusive, System.DateTime endExclusive)
        {
            return new DateRange(startInclusive, endExclusive).Contains(System.DateTime.Now);
        }

        #endregion


    }
}
