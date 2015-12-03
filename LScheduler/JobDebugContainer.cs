using LScheduler.Common;
using LScheduler.Debug.Config;
using LScheduler.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LScheduler
{
    public class JobDebugContainer : IJobDebugContainer
    {
        public List<IJobDebug> Debuggers { get; set; }

        public string Message { get; set; }

        private string _sectionName;

        private static JobDebugSection _debugRoot;

        public JobDebugContainer(string debugSectionName = null, string message = "")
        {
            _sectionName = String.IsNullOrWhiteSpace(debugSectionName) ? JobDictionary.DebugSectionName : debugSectionName;
            Message = message;
            _exceptions = new List<JobException>();
        }

        public void Init()
        {
            Debuggers = new List<IJobDebug>();
            JobDebugContainer._debugRoot = this.GetDebuggerRoot<JobDebugSection>(_sectionName);
            _exceptions.Clear();
        }

        private T GetDebuggerRoot<T>(string sectionName) where T: ConfigurationElement
        {
            return ConfigurationManager.GetSection(sectionName) as T;
        }

        private void _runAllDebuggers(string message)
        {
            foreach (IJobDebug d in Debuggers)
            {
                try
                {
                    if (!d.IsInitialized)
                    {
                        d.BeforeInit();
                    }

                    d.Debug(message);
                }
                catch (Exception ex)
                {
                    _exceptions.Add(new JobException()
                    {
                        Name = d.Name,
                        Message = ex.ToString()
                    });
                }
            }
        }

        public void Dispose()
        {
            Validate();
            foreach (IJobDebug d in Debuggers)
            {
                d.Dispose();
            }
        }

        public void InvokeALL(NameValueCollection containerParameters = null)
        {
            Validate();
            this._runAllDebuggers(Message);
        }

        public void Invoke(string name)
        {
            Validate();
            var debugger = Debuggers.Where(r => r.Name == name.Trim()).FirstOrDefault();
            try
            {
                if (!debugger.IsInitialized)
                {
                    debugger.BeforeInit();
                }
                debugger.Debug(Message);
            }
            catch (Exception ex)
            {
                _exceptions.Add(new JobException()
                {
                    Name = debugger.Name,
                    Message = ex.ToString()
                });
            }
        }

        public void Invoke<T>() where T : IJobEntity, new()
        {
            Validate();
        }

        public virtual IEnumerable<IJobEntity> Get(string name)
        {
            Validate();
            if (JobDebugContainer._debugRoot.Enabled)
            {
                var o = JobDebugContainer._debugRoot.Debuggers.GetItemByName(name);
                if (o != null)
                {
                    foreach (JobDebugConfigure config in o.DebuggerList.GetActivedItems())
                    {
                        yield return NewEntity(config, o);
                    }
                    yield break;
                }
            }
        }

        public virtual IJobEntity NewEntity(params object[] inputs)
        {
            Validate();

            var config = (JobDebugConfigure)inputs[0];
            var groupConfig = (JobDebugGroupConfigure)inputs[1];

            var t = Type.GetType(config.DebuggerType);
            var debugger = (IJobDebug)Activator.CreateInstance(t);
            debugger.Name = config.Name.Trim();
            debugger.EntityType = t;
            if (debugger.Parameters == null)
            {
                debugger.Parameters = new NameValueCollection();
            }

            //use global parameters
            if (JobDebugContainer._debugRoot.Parameters != null)
            {
                foreach (NameValueConfigurationElement nel in JobDebugContainer._debugRoot.Parameters)
                {
                    debugger.Parameters.Add(nel.Name.Trim(), nel.Value);
                }
            }

            //use group parameters
            if (groupConfig.Parameters != null)
            {
                foreach (NameValueConfigurationElement gnel in groupConfig.Parameters)
                {
                    //cover global parameters with custom parameters
                    if (debugger.Parameters.AllKeys.Contains(gnel.Name.Trim()))
                    {
                        debugger.Parameters[gnel.Name.Trim()] = gnel.Value;
                    }
                    else
                    {
                        debugger.Parameters.Add(gnel.Name.Trim(), gnel.Value);
                    }
                }
            }

            if (config.Parameters != null)
            {
                foreach (NameValueConfigurationElement nel in config.Parameters)
                {
                    //cover global parameters with custom parameters
                    if (debugger.Parameters.AllKeys.Contains(nel.Name.Trim()))
                    {
                        debugger.Parameters[nel.Name.Trim()] = nel.Value;
                    }
                    else
                    {
                        debugger.Parameters.Add(nel.Name.Trim(), nel.Value);
                    }
                }
            }

            if (config.InitAfterInstantiation)
            {
                debugger.Init();
            }

            return debugger;
        }

        public virtual IJobEntity GetOne(string name)
        {
            Validate();

            IJobEntity obj = null;
            if (JobDebugContainer._debugRoot.Enabled)
            {
                var o = JobDebugContainer._debugRoot.Debuggers;
                JobDebugGroupConfigure[] configs = new JobDebugGroupConfigure[o.Count];
                o.CopyTo(configs, 0);
                configs.ToList().ForEach(r =>
                {
                    var e = r.DebuggerList.GetItemByName(name);
                    if (e != null)
                    {
                        obj = this.NewEntity(e, r);
                        return;
                    }
                });
            }

            return obj;
        }

        public bool IsInit()
        {
            if (Debuggers != null && JobDebugContainer._debugRoot != null)
            {
                return true;
            }
            return false;
        }

        private void Validate()
        {
            if (!IsInit())
            {
                throw new Exception("Debug container has not been initialized");
            }
        }

        private List<JobException> _exceptions;
        public List<JobException> Exceptions
        {
            get
            {
                return _exceptions;
            }
        }
    }
}
