# net-lightScheduler
It is a light schedule library which is used to manage jobs in .Net solution. I created it as the idear is from my last project which is very close to payment management and I uploaded into git for learning and reminding me of what I did before. These payments that users applyed for ordering something on mobile app are not real-time deduction payment. The library is used to manage all of the jobs that are running constantly background to check the payment status in order to do the real deduction by bank's apis. It is light and easy to use and its stability is proven by the already go-live system. 
## What is net-lightScheduler
It is a .Net class library on the basis of publisher/subscriber and dependence injection design pattern and following SOLID priciple. 
## How it works
net-lightScheduler consists of jobContext, jobUnit and jobItem. All of these are able to be configured in config.
### jobContext
It contains one or more jobs. A job provides timing function to manage all of the concrete works from the start to the end. One job can be related to only on jobUnit currently.
Currently the library provides 3 native jobs to use:  
- Interval Job: the job is running under a specific timespan
- Time Point Job: the job is running under a speific time point eg. 17:00 or 8:00
- Time Job: the job is running under a specific month, date, hour, minute and second. eg. 12M means starting run job at the start of every December. 15D means starting run job at the start of 15th of every month.
### jobUnit
It is a container of jobItems. In the configuration of jobUnit, global parameters which will be passed into all of jobItems are able to be specified.
### jobItem
It is primitive of this library. jobItem does the real work according to the needs. The parameters that are needed by a particular jobItem can be set in configuration file and it also can accept global parameters set in jobUnit section.
## How to use it in your project
### configuration of jobContext
```html
<JobContext>
  <jobs>
    <job name="debtNoteTimerJob" jobType="VRentDJ.Job.PointIntervalTimerJobContainer, VRentDJ" unitName="debtNoteJob">
      <parameters>
        <!--
              M:do job at every specified month; 
              D:do job at every specified date; 
              h:do job at every specified hour; 
              m:do job at every specified minute; 
              s:do job at every specified second
              the fomat is like M or D or h followed by jobTime -->
        <add name="jobTimePattern" value="m"></add>
        <add name="jobTime" value="11"></add>
      </parameters>
    </job>
  </jobs>
</JobContext>
```
### configuration of jobUnit and jobItem
```html
<JobUnit enabled="true">
  <parameters>
    <add name="userName" value="service.center@abc.com"></add>
    <add name="userPwd" value="123456"></add>
    <add name="debitDayInterval" value="5"></add>
    <add name="debitMonth" value="0"></add>
  </parameters>
  <unit name="scheduleJob" enabled="true">
    <item name="feeDeduction" enabled="true" initAfterInstantiation="false"
       unitType="VRentDJ.Units.JobFeeDeduction, VRentDJ" order="1"></item>
    <item name="indirectFeeDeduction" enabled="true" initAfterInstantiation="false"
         unitType="VRentDJ.Units.JobIndirectFeeDeduction, VRentDJ" order="2"></item>
  </unit>
</JobUnit>
```
### Create you own jobItem
Create a derived class from class JobUnit or implement interface IJobUnit. After done it, set value of unitType to full name of your class with the correct namespace name. 
```C#
    public class JobFeeDeduction : JobUnit
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

        public override void Invoke(NameValueCollection containerParameters = null)
        {
            ProxyUserSetting setting = KemasLogin.Login(UserName, UserPwd);
            //Login user
            if (setting == null)
            {
                throw new FaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVCE000006.ToString(),
                    Message = MessageCode.CVCE000006.GetDescription(),
                    Type = ResultType.VRENT
                }, MessageCode.CVCE000006.GetDescription());
            }
            //Get uncomplished bookings
            var pbll = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            //Schedule job logic
            pbll.ScheduleJobCompletedBookings(setting, UserPwd);
        }

        public override void Init()
        {
            if (Parameters != null)
            {
                UserName = Parameters["userName"];
                UserPwd = Parameters["userPwd"];
            }
        }


        public override void Finish()
        {
        }
    }
```
### Create you own job
Create a derived class from class JobPolling or implement interface IJobPolling. After done it, create a <job></job> in jobs node in configuration file and set value of the propeties - name, type and unitName
```C#
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
                LogInfor.WriteError("Parameter is null", "Must define jobTime", "Schedule job");
            }
        }

        public override void Finish()
        {
            _running = false;
        }
```
