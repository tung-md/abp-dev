﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartSoftware.DependencyInjection;
using SmartSoftware.ExceptionHandling;
using SmartSoftware.MultiTenancy;
using SmartSoftware.Threading;

namespace SmartSoftware.BackgroundJobs;

public class BackgroundJobExecuter : IBackgroundJobExecuter, ITransientDependency
{
    public ILogger<BackgroundJobExecuter> Logger { protected get; set; }

    protected SmartSoftwareBackgroundJobOptions Options { get; }
    
    protected ICurrentTenant CurrentTenant { get; }

    public BackgroundJobExecuter(IOptions<SmartSoftwareBackgroundJobOptions> options, ICurrentTenant currentTenant)
    {
        CurrentTenant = currentTenant;
        Options = options.Value;

        Logger = NullLogger<BackgroundJobExecuter>.Instance;
    }

    public virtual async Task ExecuteAsync(JobExecutionContext context)
    {
        var job = context.ServiceProvider.GetService(context.JobType);
        if (job == null)
        {
            throw new SmartSoftwareException("The job type is not registered to DI: " + context.JobType);
        }

        var jobExecuteMethod = context.JobType.GetMethod(nameof(IBackgroundJob<object>.Execute)) ??
                               context.JobType.GetMethod(nameof(IAsyncBackgroundJob<object>.ExecuteAsync));
        if (jobExecuteMethod == null)
        {
            throw new SmartSoftwareException($"Given job type does not implement {typeof(IBackgroundJob<>).Name} or {typeof(IAsyncBackgroundJob<>).Name}. " +
                                   "The job type was: " + context.JobType);
        }

        try
        {
            using(CurrentTenant.Change(GetJobArgsTenantId(context.JobArgs)))
            {
                var cancellationTokenProvider =
                    context.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();

                using (cancellationTokenProvider.Use(context.CancellationToken))
                {
                    if (jobExecuteMethod.Name == nameof(IAsyncBackgroundJob<object>.ExecuteAsync))
                    {
                        await ((Task)jobExecuteMethod.Invoke(job, new[] { context.JobArgs })!);
                    }
                    else
                    {
                        jobExecuteMethod.Invoke(job, new[] { context.JobArgs });
                    }
                }
            }
           
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);

            await context.ServiceProvider
                .GetRequiredService<IExceptionNotifier>()
                .NotifyAsync(new ExceptionNotificationContext(ex));

            throw new BackgroundJobExecutionException("A background job execution is failed. See inner exception for details.", ex)
            {
                JobType = context.JobType.AssemblyQualifiedName!,
                JobArgs = context.JobArgs
            };
        }
    }
    
    protected virtual Guid? GetJobArgsTenantId(object jobArgs)
    {
        return jobArgs switch
        {
            IMultiTenant multiTenantJobArgs => multiTenantJobArgs.TenantId,
            _ => CurrentTenant.Id
        };
    }
}
