﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SmartSoftware.Aspects;
using SmartSoftware.AspNetCore.Filters;
using SmartSoftware.Auditing;
using SmartSoftware.DependencyInjection;

namespace SmartSoftware.AspNetCore.Mvc.Auditing;

public class SmartSoftwareAuditPageFilter : IAsyncPageFilter, ISmartSoftwareFilter, ITransientDependency
{
    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (context.HandlerMethod == null || !ShouldSaveAudit(context, out var auditLog, out var auditLogAction))
        {
            await next();
            return;
        }

        using (SmartSoftwareCrossCuttingConcerns.Applying(context.HandlerInstance, SmartSoftwareCrossCuttingConcerns.Auditing))
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await next();

                if (result.Exception != null && !auditLog!.Exceptions.Contains(result.Exception))
                {
                    auditLog!.Exceptions.Add(result.Exception);
                }
            }
            catch (Exception ex)
            {
                if (!auditLog!.Exceptions.Contains(ex))
                {
                    auditLog!.Exceptions.Add(ex);
                }
                throw;
            }
            finally
            {
                stopwatch.Stop();

                if (auditLogAction != null)
                {
                    auditLogAction.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                    auditLog!.Actions.Add(auditLogAction);
                }
            }
        }
    }

    private bool ShouldSaveAudit(PageHandlerExecutingContext context, out AuditLogInfo? auditLog, out AuditLogActionInfo? auditLogAction)
    {
        auditLog = null;
        auditLogAction = null;

        var options = context.GetRequiredService<IOptions<SmartSoftwareAuditingOptions>>().Value;
        if (!options.IsEnabled)
        {
            return false;
        }

        if (!context.ActionDescriptor.IsPageAction())
        {
            return false;
        }

        var auditLogScope = context.GetRequiredService<IAuditingManager>().Current;
        if (auditLogScope == null)
        {
            return false;
        }

        var auditingHelper = context.GetRequiredService<IAuditingHelper>();
        if (!auditingHelper.ShouldSaveAudit(context.HandlerMethod!.MethodInfo, defaultValue: true))
        {
            return false;
        }

        auditLog = auditLogScope.Log;

        if (!options.DisableLogActionInfo)
        {
            auditLogAction = auditingHelper.CreateAuditLogAction(
                auditLog,
                context.HandlerMethod.MethodInfo.DeclaringType,
                context.HandlerMethod.MethodInfo,
                context.HandlerArguments
            );
        }

        return true;
    }
}