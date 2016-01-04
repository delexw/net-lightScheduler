using LScheduler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LScheduler.Job
{
    /// <summary>
    /// Unused job
    /// </summary>
    public class PointTimerJobContainer : JobPolling
    {
        private bool _running;
        private TimeSpan startSpan;

        public override void Run()
        {
            var baseKey = new JobMessage() { Category = "01", Name = this.Name, BaseKey = Guid.NewGuid() };

            //the time at which the job will run
            var currentBaseTimePoint = DateTime.Now.Date.Add(startSpan);

            var baseDate = DateTime.Now <= currentBaseTimePoint ? currentBaseTimePoint.AddDays(-1) : currentBaseTimePoint;

            while (_running)
            {
                var nextDay = baseDate.AddDays(1);
                //current data is greater than baseDate if the job is running slowly 
                while (nextDay < DateTime.Now)
                {
                    baseDate = nextDay;
                    nextDay = nextDay.AddDays(1);
                }
                var baseTime = nextDay - baseDate;

                if (!TimePending.Pending(baseTime, baseDate, baseKey))
                {
                    if (UnitContainer != null && _running)
                    {
                        //Store the info of timer
                        if (!this.Parameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_PATTERN))
                        {
                            this.Parameters.Add(JobDictionary.JOB_INTERNAL_TIMER_PATTERN, null);
                        }
                        if (!this.Parameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_TIME))
                        {
                            this.Parameters.Add(JobDictionary.JOB_INTERNAL_TIMER_TIME, Parameters["jobTime"]);
                        }

                        UnitContainer.InvokeALL(this.Parameters);
                    }
                    baseDate = nextDay;
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                }
            }
        }

        public override void Stop()
        {
            _running = false;
        }

        public override void Init()
        {
            _running = true;

            if (Parameters != null)
            {
                TimeSpan.TryParse(Parameters["jobTime"], out startSpan);
            }
            else
            {
                _running = false;
                //LogInfor.WriteError("Parameter is null", "Must define jobTime", "Schedule job");
            }
        }

        public override void Finish()
        {
            _running = false;
        }
    }
}
