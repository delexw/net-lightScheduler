using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJobUnit : IJobGlobal, IJobEntity
    {
        bool Enabled { get; set; }
        bool IsInitialized { get; }
        IJobDebug Debug { get; set; }

        void BeforeInit();
        void Init();
        void Invoke(NameValueCollection containerParameters = null);
        void Finish();
    }
}
