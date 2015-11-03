using CF.VRent.Job.Common;
using CF.VRent.Job.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CF.VRent.Job
{
    public class JobManager
    {
        private JobContext _context;

        public Action<JobException, IJob> LogError { get; set; }

        public JobManager()     
        {
            _context = new JobContext();
        }

        public void Dispose()
        {
            try
            {
                if (JobContext.Current.CurrentJobs != null)
                {
                    foreach (IJob job in JobContext.Current.CurrentJobs)
                    {
                        job.Dispose();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString(), "Dispose");
            }
        }


        public IJob GetJob(string name)
        {
            if (JobContext.Current.CurrentJobs != null)
            {
                return JobContext.Current.CurrentJobs.Find(r => r.Name == name.Trim());
            }
            return null;
        }

        public IJob GetJob(Type type)
        {
            if (JobContext.Current.CurrentJobs != null)
            {
                return JobContext.Current.CurrentJobs.Find(r =>r.EntityType.Equals(type));
            }
            return null;
        }

        public void RunAll()
        {
            foreach (IJob job in JobContext.Current.CurrentJobs)
            {
                Task.Factory.StartNew(() =>
                {
                    //Run job
                    job.TryCatchWrapper(() =>
                    {
                        job.Run();
                    });

                    //Log the exception
                    if (job.UnitContainer.Exceptions.Count > 0)
                    {
                        foreach (JobException ex in job.UnitContainer.Exceptions)
                        {
                            LogError(ex, job);
                        }
                    }

                    if (job.DebugContainer.Exceptions.Count > 0)
                    {
                        foreach (JobException ex in job.UnitContainer.Exceptions)
                        {
                            LogError(ex, job);
                        }
                    }
                }, TaskCreationOptions.PreferFairness);
            }
        }

    }
}
