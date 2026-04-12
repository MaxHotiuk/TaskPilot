using Application.Abstractions.Archivation;
using Application.Queries.Boards;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class ArchivalBackgroundJob : IArchivalBackgroundJob
{
    private readonly IArchivalService _archivalService;
    private readonly IMediator _mediator;
    private readonly ILogger<ArchivalBackgroundJob> _logger;

    public ArchivalBackgroundJob(IArchivalService archivalService, IMediator mediator, ILogger<ArchivalBackgroundJob> logger)
    {
        _archivalService = archivalService;
        _mediator = mediator;
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

    [AutomaticRetry(Attempts = 3)]
    public async Task EnqueueStaleBoardsAsync()
    {
        try
        {
            _logger.LogInformation("Starting stale boards enqueue job");

            var staleBoardIds = await _mediator.Send(new GetStaleBoardIdsQuery());

            foreach (var boardId in staleBoardIds)
            {
                await _archivalService.MarkBoardAsArchivedAsync(boardId, "Automatically archived due to 30 days of inactivity");
            }

            _logger.LogInformation("Stale boards enqueue job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while enqueueing stale boards");
            throw;
        }
    }
}
