using Atlas;
//using Common.Logging;
using log4net;
using log4net.Config;
using System.Reflection;
using Quartz;
using Quartz.Spi;
using System.Configuration;

namespace SmartSheetWindowsService
{
    /// <summary>
    /// This represents an entity for Windows Service hosted by Atlas.
    /// </summary>
    internal class SmartSheetService : IAmAHostedProcess
    {
        //private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Gets or sets the scheduler instance.
        /// </summary>
        public IScheduler Scheduler { get; set; }           // Scheduler instance being injected through the IoC container

        /// <summary>
        /// Gets or sets the job factory instance.
        /// </summary>
        public IJobFactory JobFactory { get; set; }         // JobFactory instance being injected through the IoC container

        /// <summary>
        /// Gets or sets the job listener instance.
        /// </summary>
        public IJobListener JobListener { get; set; }       // JobListener instance being injected through the IoC container

        /// <summary>
        /// Starts the SmartSheet Windows Service.
        /// </summary>
        public void Start()
        {
            Log.Info("SmartSheet Windows Service starting");

            var job = JobBuilder.Create<SmartSheetJob>()
                                .WithIdentity("SmartSheetJob", "SmartSheetWindowsService")
                                .Build();                   // Builds the SmartSheetJob instance

            var trigger = TriggerBuilder.Create()
                                        .WithIdentity("SmartSheetTrigger", "SmartSheetWindowsService")
                                        .WithCronSchedule(ConfigurationManager.AppSettings["CronExpression"])   
                                        // The following job trigger with chron schedule will be built/run.
                                        .ForJob("SmartSheetJob", "SmartSheetWindowsService")
                                        .Build();           

            this.Scheduler.JobFactory = this.JobFactory;    // resolve the IScheduler instance
            this.Scheduler.ScheduleJob(job, trigger);       // scheduler the following job with trigger.
            this.Scheduler.ListenerManager.AddJobListener(this.JobListener);    
            this.Scheduler.Start();                         // Start the IJobScheduler instance

            Log.Info("SmartSheet Windows Service started");  // check for this in the log file
        }

        /// <summary>
        /// Stops the Windows Service.
        /// </summary>
        public void Stop()
        {
            Log.Info("SmartSheet Windows Service stopping");

            this.Scheduler.Shutdown();

            Log.Info("SmartSheet Windows Service stopped");
        }

        /// <summary>
        /// Resumes the Windows Service.
        /// </summary>
        public void Resume()
        {
            Log.Info("SmartSheet Windows Service resuming...");

            this.Scheduler.ResumeAll();

            Log.Info("SmartSheet Windows Service resumed");
        }

        /// <summary>
        /// Pauses the Windows Service.
        /// </summary>
        public void Pause()
        {
            Log.Info("SmartSheet Windows Service pausing...");

            this.Scheduler.PauseAll();

            Log.Info("SmartSheet Windows Service paused");
        }
    }
}