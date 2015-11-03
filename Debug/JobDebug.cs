using CF.VRent.Job.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CF.VRent.Job.Debug
{
    public abstract class JobDebug: IJobDebug
    {
        public bool Enabled
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

        public NameValueCollection Parameters
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

        public JobDebug()
        {
            _isInitialized = false;
            Enabled = false;
            Parameters = new NameValueCollection();
            Name = this.GetType().Name;
            EntityType = this.GetType();
        }

        public abstract void Init();

        public virtual void Debug(string message)
        {
            if (!IsInitialized)
            {
                throw new Exception("Job debug has not been initialized");
            }
        }

        public void Dispose()
        {
            Finish();
            _isInitialized = false;
        }

        public virtual void TryCatchWrapper(Action function)
        {
            try
            {
                function();
            }
            catch
            {
                this.Dispose();
            }
        }


        public void BeforeInit()
        {
            _isInitialized = true;
            Init();
        }

        public abstract void Finish();
    }
}
