using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Job.Interface
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
