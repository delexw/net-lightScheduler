using LScheduler.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LScheduler.Interface
{
    public interface IJobContainer : IJobGlobal
    {
        List<JobException> Exceptions { get; }

        void InvokeALL(NameValueCollection containerParameters = null);

        void Invoke<T>() where T : IJobEntity, new();

        void Invoke(string name);

        IEnumerable<IJobEntity> Get(string name);

        IJobEntity GetOne(string name);

        IJobEntity NewEntity(params object[] inputs);

        bool IsInit();

        void Init();
    }
}
