using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJob : IJobGlobal, IJobEntity
    {

        void Run();
        void Stop();

        DateTime? JobStartTime { get; set; }
        bool IsInitialized { get; }
        void BeforeInit();
        void Init();
        void Finish();

        IJobDebugContainer DebugContainer { get; set; }
        IJobUnitContainer UnitContainer { get; set; }
    }
}
