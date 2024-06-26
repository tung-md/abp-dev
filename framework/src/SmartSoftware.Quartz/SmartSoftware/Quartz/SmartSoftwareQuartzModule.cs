﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using SmartSoftware.Modularity;
using SmartSoftware.Threading;

namespace SmartSoftware.Quartz;

public class SmartSoftwareQuartzModule : SmartSoftwareModule
{
    private IScheduler _scheduler = default!;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var options = context.Services.ExecutePreConfiguredActions<SmartSoftwareQuartzOptions>();

        context.Services.AddQuartz(options.Properties, build =>
        {
            // these are the defaults
            if (options.Properties[StdSchedulerFactory.PropertySchedulerJobFactoryType] == null)
            {
                //MicrosoftDependencyInjectionJobFactory is the default for DI configuration, this method will be removed later on
                //build.UseMicrosoftDependencyInjectionScopedJobFactory();
            }

            if (options.Properties[StdSchedulerFactory.PropertySchedulerTypeLoadHelperType] == null)
            {
                build.UseSimpleTypeLoader();
            }

            if (options.Properties[StdSchedulerFactory.PropertyJobStoreType] == null)
            {
                build.UseInMemoryStore();
            }

            if (options.Properties[StdSchedulerFactory.PropertyThreadPoolType] == null)
            {
                build.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });
            }

            if (options.Properties["quartz.plugin.timeZoneConverter.type"] == null)
            {
                build.UseTimeZoneConverter();
            }

            options.Configurator?.Invoke(build);
        });

        context.Services.AddSingleton(serviceProvider =>
        {
            return AsyncHelper.RunSync(() => serviceProvider.GetRequiredService<ISchedulerFactory>().GetScheduler());
        });

        Configure<SmartSoftwareQuartzOptions>(quartzOptions =>
        {
            quartzOptions.Properties = options.Properties;
            quartzOptions.StartDelay = options.StartDelay;
        });
    }

    public async override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<SmartSoftwareQuartzOptions>>().Value;

        _scheduler = context.ServiceProvider.GetRequiredService<IScheduler>();

        await options.StartSchedulerFactory.Invoke(_scheduler);
    }

    public async override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        if (_scheduler.IsStarted)
        {
            await _scheduler.Shutdown();
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationShutdownAsync(context));
    }
}
