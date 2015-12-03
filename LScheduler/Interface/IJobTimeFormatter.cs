using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJobTimeFormatter
    {
        string Pattern { get; }
        string RawTime { get; }
        JobDictionary.JobTimeType Type { get; }
        DateTime FormatTime { get; }
        void Format();
    }
}
