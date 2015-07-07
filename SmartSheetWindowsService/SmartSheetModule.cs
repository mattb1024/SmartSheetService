using Atlas;
using Autofac;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Reflection;
using Module = Autofac.Module;

namespace SmartSheetWindowsService
{
    /// <summary>
    /// This is an entity for Autofac module, dealing mostly with registration of quartz components.
    /// </summary>
    internal class SmartSheetModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            this.LoadQuartz(builder);
            this.LoadServices(builder);
            this.LoadLogicLayers(builder);
        }

        /// <summary>
        /// Loads the quartz scheduler instance.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        private void LoadQuartz(ContainerBuilder builder)
        {
            builder.Register(c => new StdSchedulerFactory().GetScheduler())
                   .As<IScheduler>()
                   .InstancePerLifetimeScope(); // registering the IScheduler instance
            builder.Register(c => new SmartSheetJobFactory(ContainerProvider.Instance.ApplicationContainer))
                   .As<IJobFactory>();          // registering the IJobscheduler instance
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(p => typeof(IJob).IsAssignableFrom(p))
                   .PropertiesAutowired();      // register the IJob instance and resolve the SmartSheetJob instance
            builder.Register(c => new SmartSheetJobListener(ContainerProvider.Instance))
                   .As<IJobListener>();         // register the iJobListener instance
        }

        /// <summary>
        /// Loads the SmartSheet service instance.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        private void LoadServices(ContainerBuilder builder)
        {
            builder.RegisterType<SmartSheetService>()
                   .As<IAmAHostedProcess>()
                   .PropertiesAutowired();      // register the IAmAHostedProcess instance
        }

        /// <summary>
        /// Loads the logic layers.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        private void LoadLogicLayers(ContainerBuilder builder)
        {
            builder.RegisterType<SmartSheetLogicLayer>()
                   .As<ISmartSheetLogicLayer>();    // register the ISmartSheetLogicLayer instance
        }
    }
}