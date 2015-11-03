using CF.VRent.Job.Formatter;
using CF.VRent.Job.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CF.VRent.Job.Job
{
    public abstract class JobPolling : IJobPolling
    {
        public double JobInterval
        {
            get;
            set;
        }

        public int JobType
        {
            get;
            set;
        }

        private bool _isInitialized;
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public DateTime? JobStartTime
        {
            get;
            set;
        }

        public IJobDebugContainer DebugContainer
        {
            get;
            set;
        }

        public IJobUnitContainer UnitContainer
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Type EntityType
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public NameValueCollection Parameters
        {
            get;
            set;
        }

        public JobPolling()
        {
            _isInitialized = false;
            Parameters = new NameValueCollection();
        }

        public JobPolling(IJobDebugContainer debugger, IJobUnitContainer units)
        {
            DebugContainer = debugger;
            UnitContainer = units;
        }


        public abstract void Run();

        public abstract void Stop();

        public void Dispose()
        {
            Finish();
            if (DebugContainer != null)
            {
                DebugContainer.Dispose();
            }
            if (UnitContainer != null)
            {
                UnitContainer.Dispose();
            }
            _isInitialized = false;
        }

        public virtual void TryCatchWrapper(Action function)
        {
            try
            {
                function();
            }
            catch (Exception ex)
            {
            }
        }

        public abstract void Init();

        public void BeforeInit()
        {
            _isInitialized = true;
            Init();
        }

        public abstract void Finish();


        public IJobTimeFormatter JobTimeFormatter
        {
            get;
            set;
        }
    }
}
