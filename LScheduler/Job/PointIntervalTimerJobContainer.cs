using LScheduler.Common;
using LScheduler.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LScheduler.Job
{
    /// <summary>
    /// This job is running under a particular situation with specified time interval point defined by user in configuration file
    /// </summary>
    internal class PointIntervalTimerJobContainer : JobPolling
    {
        private bool _running;

        public override void Run()
        {
            if (DebugContainer.Debuggers == null || UnitContainer.Units == null)
            {
                throw new Exception("JobManager have not been initialized");
            }

            DateTime baseDate = new DateTime();

            if (JobTimeFormatter != null)
            {
                baseDate = JobTimeFormatter.FormatTime;
            }

            JobMessage baseKey = new JobMessage() { Category = "00", Name = this.Name, BaseKey = Guid.NewGuid() };

            while (_running)
            {
                try
                {
                    //By specified time
                    if (JobTimeFormatter != null)
                    {
                        TimeSpan baseTime = new TimeSpan();
                        switch (JobTimeFormatter.Type)
                        {
                            case JobDictionary.JobTimeType.M:
                                var nextYear = baseDate.AddYears(1);
                                //current data is greater than baseDate if the job is running slowly 
                                while (nextYear < DateTime.Now)
                                {
                                    baseDate = nextYear;
                                    nextYear = nextYear.AddYears(1);
                                }
                                baseTime = nextYear - baseDate;
                                if (!TimePending.Pending(baseTime, baseDate, baseKey))
                                {
                                    _run();
                                    //Prepare for the next plan
                                    baseDate = nextYear;
                                }
                                break;
                            case JobDictionary.JobTimeType.D:
                                var nextMonth = baseDate.AddMonths(1);
                                //current data is greater than baseDate if the job is running slowly 
                                while (nextMonth < DateTime.Now)
                                {
                                    baseDate = nextMonth;
                                    nextMonth = nextMonth.AddMonths(1);
                                }
                                baseTime = nextMonth - baseDate;
                                if (!TimePending.Pending(baseTime, baseDate, baseKey))
                                {
                                    _run();
                                    baseDate = nextMonth;
                                }
                                break;
                            case JobDictionary.JobTimeType.h:
                                var nexDay = baseDate.AddDays(1);
                                //current data is greater than baseDate if the job is running slowly 
                                while (nexDay < DateTime.Now)
                                {
                                    baseDate = nexDay;
                                    nexDay = nexDay.AddDays(1);
                                }
                                baseTime = nexDay - baseDate;
                                if (!TimePending.Pending(baseTime, baseDate, baseKey))
                                {
                                    _run();
                                    baseDate = nexDay;
                                }
                                break;
                            case JobDictionary.JobTimeType.m:
                                var nextHour = baseDate.AddHours(1);
                                //current data is greater than baseDate if the job is running slowly 
                                while (nextHour < DateTime.Now)
                                {
                                    baseDate = nextHour;
                                    nextHour = nextHour.AddHours(1);
                                }
                                baseTime = nextHour - baseDate;
                                if (!TimePending.Pending(baseTime, baseDate, baseKey))
                                {
                                    _run();
                                    baseDate = nextHour;
                                }
                                break;
                            case JobDictionary.JobTimeType.s:
                                var nextMinute = baseDate.AddMinutes(1);
                                //current data is greater than baseDate if the job is running slowly 
                                while (nextMinute < DateTime.Now)
                                {
                                    baseDate = nextMinute;
                                    nextMinute = nextMinute.AddMinutes(1);
                                }
                                baseTime = nextMinute - baseDate;
                                if (!TimePending.Pending(baseTime, baseDate, baseKey))
                                {
                                    _run();
                                    baseDate = nextMinute;
                                }
                                break;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void _run()
        {
            if (UnitContainer != null && _running)
            {
                //Store the info of timer
                if (!this.Parameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_PATTERN))
                {
                    this.Parameters.Add(JobDictionary.JOB_INTERNAL_TIMER_PATTERN, JobTimeFormatter.Pattern);
                }
                if (!this.Parameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_TIME))
                {
                    this.Parameters.Add(JobDictionary.JOB_INTERNAL_TIMER_TIME, JobTimeFormatter.RawTime);
                }

                UnitContainer.InvokeALL(this.Parameters);
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
        }

        public override void Stop()
        {
            _running = false;
        }

        public override void Init()
        {
            _running = true;

            if (Parameters["jobTimePattern"] != null && Parameters["jobTime"] != null)
            {
                JobTimeFormatter = new JobTimeFormatter(Parameters["jobTimePattern"].ToString(), Parameters["jobTime"].ToString());
                JobTimeFormatter.Format();
            }
            else
            {
                _running = false;
                //LogInfor.WriteError("Parameter is null", "Must define jobTimePattern and jobTime", "Schedule job");
            }
        }

        public override void Finish()
        {
            _running = false;
        }
    }
}
