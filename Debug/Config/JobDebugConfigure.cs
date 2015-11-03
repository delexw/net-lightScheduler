using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.Job.Debug.Config
{
    public class JobDebugSection : ConfigurationSection
    {
        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)base["enabled"];
            }
            set
            {
                base["enabled"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(JobDebugGroupConfigure), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, RemoveItemName = "remove", AddItemName = "debugger")]
        public JobDebugGroupConfigureCollection Debuggers
        {
            get
            {
                return (JobDebugGroupConfigureCollection)base[""];
            }
            set
            {
                base[""] = value;
            }
        }

        [ConfigurationProperty("parameters", IsRequired = false, IsDefaultCollection = false)]
        public NameValueConfigurationCollection Parameters
        {
            get
            {
                return (NameValueConfigurationCollection)base["parameters"];
            }
            set
            {
                base["parameters"] = value;
            }
        }
    }

    /// <summary>
    /// Debugger Group
    /// </summary>
    public class JobDebugGroupConfigure : ConfigurationElement
    {
        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)base["enabled"];
            }
            set
            {
                base["enabled"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return base["name"].ToString().Trim();
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(JobDebugConfigure), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, RemoveItemName = "remove", AddItemName = "item")]
        public JobDebugConfigureCollection DebuggerList
        {
            get
            {
                return (JobDebugConfigureCollection)base[""];
            }
            set
            {
                base[""] = value;
            }
        }

        [ConfigurationProperty("parameters", IsRequired = false, IsDefaultCollection = false)]
        public NameValueConfigurationCollection Parameters
        {
            get
            {
                return (NameValueConfigurationCollection)base["parameters"];
            }
            set
            {
                base["parameters"] = value;
            }
        }
    }

    /// <summary>
    /// Debugger
    /// </summary>
    public class JobDebugConfigure : ConfigurationElement
    {
        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)base["enabled"];
            }
            set
            {
                base["enabled"] = value;
            }
        }

        [ConfigurationProperty("debuggerType", IsRequired = true)]
        public string DebuggerType
        {
            get
            {
                return (string)base["debuggerType"];
            }
            set
            {
                base["debuggerType"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return base["name"].ToString().Trim();
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("initAfterInstantiation", IsRequired = false, DefaultValue = false)]
        public bool InitAfterInstantiation
        {
            get
            {
                return (bool)base["initAfterInstantiation"];
            }
            set
            {
                base["initAfterInstantiation"] = value;
            }
        }

        [ConfigurationProperty("order", IsRequired = false)]
        public int Order
        {
            get
            {
                return (int)base["order"];
            }
            set
            {
                base["order"] = value;
            }
        }

        [ConfigurationProperty("parameters", IsRequired = false, IsDefaultCollection = false)]
        public NameValueConfigurationCollection Parameters
        {
            get
            {
                return (NameValueConfigurationCollection)base["parameters"];
            }
            set
            {
                base["parameters"] = value;
            }
        }
    }

    /// <summary>
    /// Debugger Group Collection
    /// </summary>
    public class JobDebugGroupConfigureCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JobDebugGroupConfigure();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobDebugGroupConfigure)element).Name;
        }

        public JobDebugGroupConfigure GetItemByName(string name)
        {
            var o = (JobDebugGroupConfigure)base.BaseGet(name);
            return o.Enabled ? o : null;
        }
    }

    /// <summary>
    /// Debugger Collection
    /// </summary>
    public class JobDebugConfigureCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JobDebugConfigure();
        }

        public JobDebugConfigure this[int i]
        {
            get
            {
                return (JobDebugConfigure)base.BaseGet(i);
            }
        }

        //public JobDebugConfigure this[string name]
        //{
        //    get
        //    {
        //        ConfigurationElement[] all = new ConfigurationElement[base.Count];
        //        base.CopyTo(all, 0);
        //        var el = all.Where(a => a.ElementInformation.Properties["name"].Value.ToString().Trim() == name.Trim()).FirstOrDefault();
        //        if (el != null)
        //        {
        //            return (JobDebugConfigure)el;
        //        }
        //        return null;
        //    }
        //}

        public IEnumerable<JobDebugConfigure> GetItems()
        {
            var ite = base.GetEnumerator();

            while (ite.MoveNext())
            {
                yield return (JobDebugConfigure)ite.Current;
            }
            yield break;
        }

        public IEnumerable<JobDebugConfigure> GetActivedItems()
        {
            return this.GetItems().Where(r => r.Enabled).OrderBy(r=>r.Order).AsEnumerable();
        }

        public JobDebugConfigure GetItemByName(string name)
        {
            var o = (JobDebugConfigure)base.BaseGet(name);
            return o.Enabled ? o : null;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobDebugConfigure)element).Name;
        }
    }
}
