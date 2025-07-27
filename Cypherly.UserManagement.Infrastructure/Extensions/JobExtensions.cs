﻿using System.Reflection;
using Cypherly.UserManagement.Infrastructure.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Cypherly.UserManagement.Infrastructure.Extensions;

internal static class JobExtensions
{
    internal static void AddOutboxProcessingJob(this IServiceCollection services, Assembly assembly)
    {
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey($"{nameof(ProcessOutboxMessageJob)}-{assembly.GetName()}");

            configure.AddJob<ProcessOutboxMessageJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()));
        });

        services.AddQuartzHostedService();
    }
}