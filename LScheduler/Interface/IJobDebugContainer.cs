using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJobDebugContainer : IJobContainer
    {
        List<IJobDebug> Debuggers { get; set; }

        string Message { get; set; }
    }
}
