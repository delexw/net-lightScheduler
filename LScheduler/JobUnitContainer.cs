using LScheduler.Common;
using LScheduler.Interface;
using LScheduler.Units.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LScheduler
{
    public class JobUnitContainer : IJobUnitContainer
    {
        public List<IJobUnit> Units { get; set; }

        private string _sectionName;

        private static JobUnitSection _unitRoot;

        private IJobDebugContainer _debugContainer;

        public JobUnitContainer(string unitSectionName = null)
        {
            _sectionName = String.IsNullOrWhiteSpace(unitSectionName) ? JobDictionary.UnitSectionName : unitSectionName;
            _exceptions = new List<JobException>();
        }

        public void Init()
        {
            Units = new List<IJobUnit>();
            JobUnitContainer._unitRoot = this.GetUnitRoot<JobUnitSection>(_sectionName);
            _exceptions.Clear();
        }

        private T GetUnitRoot<T>(string sectionName) where T : ConfigurationElement
        {
            return ConfigurationManager.GetSection(sectionName) as T;
        }

        public void SetDebugContainer(IJobDebugContainer dc)
        {
            _debugContainer = dc;
        }

        public void InvokeALL(NameValueCollection containerParameters = null)
        {
            Validate();
            _exceptions.Clear();
            foreach (IJobUnit u in Units)
            {
                try
                {
                    try
                    {
                        if (!u.IsInitialized)
                        {
                            u.BeforeInit();
                        }
                        if (u.Debug != null)
                        {
                            if (!u.Debug.IsInitialized)
                            {
                                u.Debug.BeforeInit();
                            }
                            u.Debug.Debug("(" + u.Name + ")Run job at " + DateTime.Now.ToString());
                        }
                    }
                    catch
                    {
                        throw;
                    }

                    u.TryCatchWrapper(() => {
                        u.Invoke(containerParameters);
                    });

                    try
                    {
                        if (u.Debug != null)
                        {
                            if (!u.Debug.IsInitialized)
                            {
                                u.Debug.BeforeInit();
                            }
                            u.Debug.Debug("(" + u.Name + ")Finished job at " + DateTime.Now.ToString());
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _exceptions.Add(new JobException()
                    {
                        Name = u.Name,
                        Message = ex.ToString()
                    });
                }
            }
        }

        public void Invoke<T>() where T : IJobEntity, new()
        {
            Validate();
        }

        public void Invoke(string name)
        {
            Validate();
        }

        public void Dispose()
        {
            Validate();
            foreach (IJobUnit u in Units)
            {
                u.Dispose();
            }
        }

        /// <summary>
        /// Get items by name
        /// </summary>
        /// <param name="name">Group name</param>
        /// <returns></returns>
        public virtual IEnumerable<IJobEntity> Get(string name)
        {
            Validate();
            if (JobUnitContainer._unitRoot.Enabled)
            {
                var o = JobUnitContainer._unitRoot.Units.GetItemByName(name);
                if (o != null)
                {
                    foreach (JobUnitConfigure config in o.UnitList.GetActivedItems())
                    {
                        yield return NewEntity(config, o);
                    }
                    yield break;
                }
            }
        }

        /// <summary>
        /// New entity
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public virtual IJobEntity NewEntity(params object[] inputs)
        {
            Validate();

            var config = (JobUnitConfigure)inputs[0];
            var groupConfig = (JobUnitGroupConfigure)inputs[1];

            var t = Type.GetType(config.DebuggerType);
            var unit = (IJobUnit)Activator.CreateInstance(t);
            unit.Name = config.Name;
            unit.EntityType = t;

            if (unit.Parameters == null)
            {
                unit.Parameters = new NameValueCollection();
            }

            //use global parameters
            if (JobUnitContainer._unitRoot.Parameters != null)
            {
                foreach (NameValueConfigurationElement nel in JobUnitContainer._unitRoot.Parameters)
                {
                    unit.Parameters.Add(nel.Name.Trim(), nel.Value);
                }
            }

            //use group parameters
            if (groupConfig.Parameters != null)
            {
                foreach (NameValueConfigurationElement gnel in groupConfig.Parameters)
                {
                    //cover global parameters with custom parameters
                    if (unit.Parameters.AllKeys.Contains(gnel.Name.Trim()))
                    {
                        unit.Parameters[gnel.Name.Trim()] = gnel.Value;
                    }
                    else
                    {
                        unit.Parameters.Add(gnel.Name.Trim(), gnel.Value);
                    }
                }
            }

            if (config.Parameters != null)
            {
                foreach (NameValueConfigurationElement nel in config.Parameters)
                {
                    //cover global parameters with custom parameters
                    if (unit.Parameters.AllKeys.Contains(nel.Name.Trim()))
                    {
                        unit.Parameters[nel.Name.Trim()] = nel.Value;
                    }
                    else
                    {
                        unit.Parameters.Add(nel.Name.Trim(), nel.Value);
                    }
                }
            }

            //get relative debugger
            if (config.DebuggerItemName != null && _debugContainer != null)
            {
                unit.Debug = (IJobDebug)_debugContainer.GetOne(config.DebuggerItemName);
            }


            if (config.InitAfterInstantiation)
            {
                unit.Init();
            }

            return unit;
        }

        /// <summary>
        /// Get unit item
        /// </summary>
        /// <param name="name">Item name</param>
        /// <returns></returns>
        public virtual IJobEntity GetOne(string name)
        {
            Validate();
            IJobEntity obj = null;
            if (JobUnitContainer._unitRoot.Enabled)
            {
                var o = JobUnitContainer._unitRoot.Units;
                JobUnitGroupConfigure[] configs = new JobUnitGroupConfigure[o.Count];
                o.CopyTo(configs, 0);
                configs.ToList().ForEach(r =>
                {
                    var e = r.UnitList.GetItemByName(name);
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
            if (Units != null && JobUnitContainer._unitRoot != null)
            {
                return true;
            }
            return false;
        }

        private void Validate()
        {
            if (!IsInit())
            {
                throw new Exception("Unit container has not been initialized");
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
