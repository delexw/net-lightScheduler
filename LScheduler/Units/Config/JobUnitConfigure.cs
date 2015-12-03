using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LScheduler.Units.Config
{
    public class JobUnitSection : ConfigurationSection
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
        [ConfigurationCollection(typeof(JobUnitGroupConfigure), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, RemoveItemName = "remove", AddItemName = "unit")]
        public JobUnitGroupConfigureCollection Units
        {
            get
            {
                return (JobUnitGroupConfigureCollection)base[""];
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

    public class JobUnitGroupConfigure : ConfigurationElement
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
        [ConfigurationCollection(typeof(JobUnitConfigure), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, RemoveItemName = "remove", AddItemName = "item")]
        public JobUnitConfigureCollection UnitList
        {
            get
            {
                return (JobUnitConfigureCollection)base[""];
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

    public class JobUnitConfigure : ConfigurationElement
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

        [ConfigurationProperty("unitType", IsRequired = true)]
        public string DebuggerType
        {
            get
            {
                return (string)base["unitType"];
            }
            set
            {
                base["unitType"] = value;
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

        [ConfigurationProperty("debuggerItemName", IsRequired = false)]
        public string DebuggerItemName
        {
            get
            {
                return base["debuggerItemName"].ToString().Trim();
            }
            set
            {
                base["debuggerItemName"] = value;
            }
        }
    }

    public class JobUnitGroupConfigureCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JobUnitGroupConfigure();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobUnitGroupConfigure)element).Name;
        }

        public JobUnitGroupConfigure GetItemByName(string name)
        {
            var o = (JobUnitGroupConfigure)base.BaseGet(name);
            return o.Enabled ? o : null;
        }
    }

    public class JobUnitConfigureCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JobUnitConfigure();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobUnitConfigure)element).Name;
        }

        public JobUnitConfigure this[int i]
        {
            get
            {
                return (JobUnitConfigure)base.BaseGet(i);
            }
        }

        public JobUnitConfigure GetItemByName(string name)
        {
            var o = (JobUnitConfigure)base.BaseGet(name);
            return o.Enabled ? o : null;
        }

        public IEnumerable<JobUnitConfigure> GetItems()
        {
            var ite = base.GetEnumerator();

            while (ite.MoveNext())
            {
                yield return (JobUnitConfigure)ite.Current;
            }
            yield break;
        }

        public IEnumerable<JobUnitConfigure> GetActivedItems()
        {
            return this.GetItems().Where(r => r.Enabled).OrderBy(r => r.Order).AsEnumerable();
        }
    }
}
