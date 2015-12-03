using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler.Common
{
    public class JobException : Exception
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
