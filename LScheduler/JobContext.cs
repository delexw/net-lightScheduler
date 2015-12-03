using LScheduler.Interface;
using LScheduler.Job.Config;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LScheduler
{
    public sealed class JobContext
    {
        private static JobContext _current;
        public static JobContext Current
        {
            get {
            return _current;
        } }

        public List<IJob> CurrentJobs
        {
            get
            {
                return _currentJob;
            }
            private set
            {
                _currentJob = value;
            }
        }

        private List<IJob> _currentJob;

        public JobContext()
        {
            _currentJob = this._getJobs().ToList();
            //_currentJob.DebugContainer = debugger;
            //_currentJob.UnitContainer = unit;
            _current = this;
        }

        /// <summary>
        /// Get jobs
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IJob> _getJobs()
        {
            var jobRoot = ConfigurationManager.GetSection(JobDictionary.ContextSectionrName) as JobContextSection;

            if (jobRoot == null)
            {
                throw new Exception("Section name is incorrect. The section name for context is JobContainer");
            }

            List<JobContainerConfigure> queryJobConfigues = new List<JobContainerConfigure>();
            List<IJob> queryJobs = new List<IJob>();
            foreach (JobContainerConfigure jc in jobRoot.Jobs)
            {
                queryJobConfigues.Add(jc);
            }

            //Initiate the context and containers
            Parallel.ForEach<JobContainerConfigure>(queryJobConfigues, jc =>
            {
                var t = Type.GetType(jc.JobType);

                var job = (IJob)Activator.CreateInstance(t);

                job.Name = jc.Name;
                job.EntityType = t;

                if (job.Parameters == null)
                {
                    job.Parameters = new NameValueCollection();
                }

                //use global parameters
                if (jobRoot.Parameters != null)
                {
                    foreach (NameValueConfigurationElement nel in jobRoot.Parameters)
                    {
                        job.Parameters.Add(nel.Name.Trim(), nel.Value);
                    }
                }

                if (jc.Parameters != null)
                {
                    foreach (NameValueConfigurationElement nel in jc.Parameters)
                    {
                        //cover global parameters with custom parameters
                        if (job.Parameters.AllKeys.Contains(nel.Name.Trim()))
                        {
                            job.Parameters[nel.Name.Trim()] = nel.Value;
                        }
                        else
                        {
                            job.Parameters.Add(nel.Name.Trim(), nel.Value);
                        }
                    }
                }

                //Get units
                if (!String.IsNullOrEmpty(jc.UnitName))
                {
                    job.DebugContainer = new JobDebugContainer();
                    job.DebugContainer.Init();

                    job.UnitContainer = new JobUnitContainer();
                    job.UnitContainer.Init();
                    job.UnitContainer.SetDebugContainer(job.DebugContainer);
                    job.UnitContainer.Units = job.UnitContainer.Get(jc.UnitName).Cast<IJobUnit>().ToList();

                    foreach (IJobUnit ju in job.UnitContainer.Units)
                    {
                        if (ju.Debug != null && job.DebugContainer.Debuggers.FirstOrDefault(r => r.Name == ju.Debug.Name) == null)
                        {
                            job.DebugContainer.Debuggers.Add(ju.Debug);
                        }
                    }
                }

                job.BeforeInit();

                queryJobs.Add(job);
            });

            return queryJobs;
        }
    }
}
