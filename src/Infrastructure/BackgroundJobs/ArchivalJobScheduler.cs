using Application.Abstractions.Archivation;
using Hangfire;

namespace Infrastructure.BackgroundJobs;

public class ArchivalJobScheduler : IArchivalJobScheduler
{
    public void ScheduleRecurringJobs()
    {
        RecurringJob.AddOrUpdate<ArchivalBackgroundJob>(
            "process-archived-boards",
            job => job.ProcessArchivedBoardsAsync(),
            "*/5 * * * *");
    }
}