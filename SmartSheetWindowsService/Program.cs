using Atlas;
using Autofac;
//using Common.Logging;
using log4net;
using log4net.Config;
using System;
using System.Linq;
using System.Reflection;

namespace SmartSheetWindowsService
{
    /// <summary>
    /// This represents the Windows Service application entity.
    /// </summary>
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This represents the main entry point of the Windows Service application.
        /// </summary>
        /// <param name="args">List of arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();

                var configuration = Host.UseAppConfig<SmartSheetService>()  // #1
                                        .AllowMultipleInstances()       // #2
                                        .WithRegistrations(p => p.RegisterModule(new SmartSheetModule()));  // #3
                if (args != null && args.Any())
                    configuration = configuration.WithArguments(args);  // #4

                Host.Start(configuration);                              // #5
            }
            catch (Exception ex)
            {
                Log.Fatal("Exception during startup.", ex);
                Console.Read();
            }
        }
    }
}