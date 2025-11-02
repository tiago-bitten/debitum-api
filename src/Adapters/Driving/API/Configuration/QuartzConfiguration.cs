using API.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace API.Configuration;

public static class QuartzConfiguration
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(configurator =>
        {
            configurator.UseInMemoryStore();

            var debtReminderJobKey = new JobKey(nameof(DebtReminderJob));

            configurator.AddJob<DebtReminderJob>(opts => opts
                .WithIdentity(debtReminderJobKey)
                .WithDescription("Daily job to check debts needing reminders and queue messages"));

            configurator.AddTrigger(opts => opts
                .ForJob(debtReminderJobKey)
                .WithIdentity($"{nameof(DebtReminderJob)}-trigger")
                .WithCronSchedule("0 0 9 * * ?")
                .WithDescription("Runs daily to check debts needing reminders")
                .StartNow());
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
            options.AwaitApplicationStarted = true;
        });

        return services;
    }
}
