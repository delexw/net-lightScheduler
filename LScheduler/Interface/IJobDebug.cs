using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJobDebug : IJobGlobal,IJobEntity
    {
        bool Enabled { get; set; }
        bool IsInitialized { get; }

        void BeforeInit();
        void Init();
        void Debug(string message);
        void Finish();
    }
}
