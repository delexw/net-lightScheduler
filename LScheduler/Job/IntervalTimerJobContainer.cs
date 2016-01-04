using LScheduler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LScheduler.Job
{
    /// <summary>
    /// This job is ruuning under a particular situation with a specified time interval defined by user in configuration file 
    /// </summary>
    internal class IntervalTimerJobContainer : JobPolling
    {
        private Timer t;
        private double it;
        JobMessage _baseKey;

        public IntervalTimerJobContainer()
            : base(null, null)
        {
        }

        public override void Run()
        {
            if (DebugContainer.Debuggers == null || UnitContainer.Units == null)
            {
                throw new Exception("JobManager have not been initialized");
            }

            //UnitContainer.InvokeALL();

            if (t != null)
            {
                t.Start();

                _baseKey = new JobMessage() { Category = "02", Name = this.Name, BaseKey = Guid.NewGuid() };

                //LogInfor.GetLogger<TimerLogger>().WriteInfo(_baseKey.Format() + " - Left time[" + (t.Interval/100).ToString() + "s]", "", "");
            }
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //LogInfor.GetLogger<TimerLogger>().WriteInfo(_baseKey.Format() + " - Start job", "", "");

                if (UnitContainer != null)
                {
                    //Store the info of timer
                    if (!this.Parameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_PATTERN))
                    {
                        this.Parameters.Add(JobDictionary.JOB_INTERNAL_TIMER_PATTERN, null);
                    }
                    if (!this.Parameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_TIME))
                    {
                        this.Parameters.Add(JobDictionary.JOB_INTERNAL_TIMER_TIME, t.Interval.ToString());
                    }

                    UnitContainer.InvokeALL(this.Parameters);
                }
            }
            catch
            {
            }
            finally
            {
                //Collect garbage manually
                GC.Collect();
                GC.WaitForFullGCComplete();
                //Raise event manually
                t.Enabled = true;
                //LogInfor.GetLogger<TimerLogger>().WriteInfo(_baseKey.Format() + " - Left time[" + (t.Interval / 100).ToString() + "s]", "", "");
            }
        }


        public override void Finish()
        {
            if (t != null)
            {
                t.Stop();
                t.Close();
            }
        }

        public override void Stop()
        {
            if (t != null && t.Enabled)
            {
                t.Stop();
            }
        }

        public override void Init()
        {
            if (Parameters != null)
            {
                it = Convert.ToDouble(Parameters["jobTime"]) * 60 * 1000;
                t = new System.Timers.Timer(it);

                if (it > 0)
                {
                    //Don't reset the singlal for raising event
                    t.AutoReset = false;
                    t.Elapsed += t_Elapsed;
                }
            }
            else
            {
                //LogInfor.WriteError("Parameter is null", "Must define jobTime", "Schedule job");
            }
        }
    }
}
