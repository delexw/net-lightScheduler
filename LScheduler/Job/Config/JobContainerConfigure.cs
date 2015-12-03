using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LScheduler.Job.Config
{
    public class JobContextSection : ConfigurationSection
    {
        [ConfigurationProperty("jobs", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(JobContainerConfigure), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, RemoveItemName = "remove", AddItemName = "job")]
        public JobContainerConfigureCollection Jobs
        {
            get
            {
                return (JobContainerConfigureCollection)base["jobs"];
            }
            set
            {
                base["jobs"] = value;
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

    public class JobContainerConfigure : ConfigurationElement
    {
        [ConfigurationProperty("jobType", IsRequired = true)]
        public string JobType
        {
            get
            {
                return (string)base["jobType"];
            }
            set
            {
                base["jobType"] = value;
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

        [ConfigurationProperty("unitName", IsRequired = true)]
        public string UnitName
        {
            get
            {
                return base["unitName"].ToString().Trim();
            }
            set
            {
                base["unitName"] = value;
            }
        }
    }

    public class JobContainerConfigureCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JobContainerConfigure();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobContainerConfigure)element).Name;
        }
    }
}
