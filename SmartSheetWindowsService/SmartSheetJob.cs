//using Common.Logging;
using Quartz;
using log4net;
using log4net.Appender;
using System.Reflection;

namespace SmartSheetWindowsService
{
    /// <summary>
    /// This represents an entity for the job that actually performs for the schedule.
    /// </summary>
    internal class SmartSheetJob : IJob
    {
        //private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public ISmartSheetLogicLayer SmartSheetLogicLayer { get; set; }

        /// <summary>
        /// Called by the <c>Quartz.IScheduler</c> when a <c>Quartz.ITrigger</c> fires that is associated with the <c>Quartz.IJob</c>.
        /// </summary>
        /// <param name="context">JobExecutionContext instance</param>
        public void Execute(IJobExecutionContext context)
        {
            Log.Info("Job executing");

            this.SmartSheetLogicLayer.Run();

            Log.Info("Job executed");
        }
    }
}