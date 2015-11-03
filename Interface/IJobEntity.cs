using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CF.VRent.Job.Interface
{
    public interface IJobEntity
    {
        string Name { get; set; }
        Type EntityType { get; set; }
        int Order { get; set; }
        NameValueCollection Parameters { get; set; }

        void TryCatchWrapper(Action function);
    }
}
