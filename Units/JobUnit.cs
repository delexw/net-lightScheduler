using CF.VRent.Job.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Job.Units
{
    public abstract class JobUnit: IJobUnit
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

        public IJobDebug Debug
        {
            get;
            set;
        }

        public JobUnit()
        {
            _isInitialized = false;
            Enabled = false;
            Parameters = new NameValueCollection();
            Name = this.GetType().Name;
            EntityType = this.GetType();
        }

        public abstract void Init();

        public abstract void Invoke(NameValueCollection containerParameters = null);

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
            catch (FaultException fe)
            {
                Type[] type = fe.GetType().GetGenericArguments();
                if (type.Length == 1)
                {
                    var value = fe.GetType().GetProperty("Detail").GetValue(fe, null);
                    var message = value.GetType().GetProperty("Message").GetValue(value, null).ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
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
