using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJobPolling : IJob
    {
        double JobInterval { get; set; }
        int JobType { get; set; }
        IJobTimeFormatter JobTimeFormatter { get; set; }
    }
}
