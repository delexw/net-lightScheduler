using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LScheduler.Common
{
    internal class JobMessage
    {
        public string Category { get; set; }

        public Guid BaseKey { get; set; }

        public string Name { get; set; }

        public string Format()
        {
            return Category + "|" + Name + "|" + BaseKey.ToString("N");
        }
    }
}
