using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Job.Interface
{
    public interface IJobUnitContainer : IJobContainer
    {
        List<IJobUnit> Units { get; set; }

        void SetDebugContainer(IJobDebugContainer dc);
    }
}
