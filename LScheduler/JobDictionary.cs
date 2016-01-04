using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler
{
    public class JobDictionary
    {
        public enum JobTimeType
        {
            /// <summary>
            /// Do job at every specified month
            /// </summary>
            M = 1,
            /// <summary>
            /// Do job at every specified date
            /// </summary>
            D = 2,
            /// <summary>
            /// Do job at every specified hour
            /// </summary>
            h = 3,
            /// <summary>
            /// Do job at every specified minute
            /// </summary>
            m = 4,
            /// <summary>
            /// Do job at every specified second
            /// </summary>
            s = 5
        }

        public const string DebugSectionName = "JobDebugger";
        public const string UnitSectionName = "JobUnit";
        public const string ContextSectionrName = "JobContext";

        public const string JOB_INTERNAL_TIMER_PATTERN = "JOB_INTERNAL_TIMER_PATTERN";
        public const string JOB_INTERNAL_TIMER_TIME = "JOB_INTERNAL_TIMER_TIME";
    }
}
