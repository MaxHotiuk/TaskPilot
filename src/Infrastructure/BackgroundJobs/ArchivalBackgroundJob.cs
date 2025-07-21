using Application.Abstractions.Archivation;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class ArchivalBackgroundJob : IArchivalBackgroundJob
{
    private readonly IArchivalService _archivalService;
    private readonly ILogger<ArchivalBackgroundJob> _logger;

    public ArchivalBackgroundJob(IArchivalService archivalService, ILogger<ArchivalBackgroundJob> logger)
    {
        _archivalService = archivalService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessArchivedBoardsAsync()
    {
        try
        {
            _logger.LogInformation("Starting archived boards processing job");
            await _archivalService.ProcessArchivedBoardsAsync();
            _logger.LogInformation("Archived boards processing job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing archived boards");
            throw;
        }
    }
}