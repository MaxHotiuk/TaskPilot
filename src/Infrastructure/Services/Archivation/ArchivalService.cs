using System.Text.Json;
using Application.Abstractions.Archivation;
using Domain.Dtos.Boards;
using Domain.Entities;
using MediatR;
using Application.Commands.Boards;
using Application.Commands.ArchivalJobs;
using Application.Queries.ArchivalJobs;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Domain.Enums;

namespace Infrastructure.Services.Archivation;


public class ArchivalService : IArchivalService, IDisposable
{
    private readonly IMediator _mediator;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<ArchivalService> _logger;
    private readonly string _queueName;

    public ArchivalService(
        IMediator mediator,
        ServiceBusClient serviceBusClient,
        IConfiguration configuration,
        ILogger<ArchivalService> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _queueName = configuration["ServiceBus:ArchivalQueueName"] ?? "board-archival-queue";
        _sender = serviceBusClient.CreateSender(_queueName);
    }

    public async Task<ArchivalJobDto> MarkBoardAsDearchivedAsync(Guid boardId, string? dearchivalReason = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking board {BoardId} as dearchived", boardId);

            var pendingArchivalJobs = await _mediator.Send(new GetPendingArchivalJobsQuery(), cancellationToken);
            var archivalJobForBoard = pendingArchivalJobs?.FirstOrDefault(j => j.BoardId == boardId);
            if (archivalJobForBoard != null)
            {
                _logger.LogInformation("Board {BoardId} is in the archival queue. Removing pending archival job {JobId}.", boardId, archivalJobForBoard.Id);
                await _mediator.Send(new RemoveArchivalJobCommand(
                    JobId: archivalJobForBoard.Id
                ), cancellationToken);

                return archivalJobForBoard;
            }

            await _mediator.Send(new UpdateBoardArchivalStatusCommand(
                BoardId: boardId,
                IsArchived: false,
                ArchivedAt: null,
                ArchivalReason: dearchivalReason
            ), cancellationToken);

            var job = await _mediator.Send(new CreateArchivalJobCommand(
                BoardId: boardId,
                JobType: "BoardDearchival",
                Metadata: dearchivalReason != null ? JsonSerializer.Serialize(new { DearchivalReason = dearchivalReason }) : null
            ), cancellationToken);

            _logger.LogInformation("Created dearchival job {JobId} for board {BoardId}", job.Id, boardId);

            await ProcessDearchivedBoardsAsync(cancellationToken);

            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking board {BoardId} as dearchived", boardId);
            throw;
        }
    }

    public async Task ProcessDearchivedBoardsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting to process dearchived boards");

            var pendingJobs = await _mediator.Send(new GetDearchivalJobsForProcessingQuery(), cancellationToken);

            _logger.LogInformation("Found {Count} pending dearchival jobs", pendingJobs.Count());

            foreach (var job in pendingJobs)
            {
                try
                {
                    await _mediator.Send(new UpdateArchivalJobStatusCommand(
                        JobId: job.Id,
                        Status: (int)ArchivalStatus.InProgress,
                        ErrorMessage: null,
                        ProcessedBy: "ArchivalService"
                    ), cancellationToken);

                    var success = await EnqueueDearchivedBoardAsync(job.BoardId, job.Id, cancellationToken);

                    if (success)
                    {
                        _logger.LogInformation("Successfully enqueued dearchival for board {BoardId} with job {JobId}", job.BoardId, job.Id);
                    }
                    else
                    {
                        await _mediator.Send(new FailArchivalJobCommand(
                            JobId: job.Id,
                            ErrorMessage: "Failed to enqueue dearchival message to Service Bus",
                            ProcessedBy: "ArchivalService"
                        ), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing dearchival job {JobId}", job.Id);

                    await _mediator.Send(new FailArchivalJobCommand(
                        JobId: job.Id,
                        ErrorMessage: ex.Message,
                        ProcessedBy: "ArchivalService"
                    ), cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessDearchivedBoardsAsync");
            throw;
        }
    }

    public async Task<bool> EnqueueDearchivedBoardAsync(Guid boardId, Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            var dearchivalMessage = new ArchivalMessage
            {
                BoardId = boardId,
                JobId = jobId,
                JobType = "BoardDearchival",
                EnqueuedAt = DateTime.UtcNow
            };

            var messageBody = JsonSerializer.Serialize(dearchivalMessage);
            var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = jobId.ToString(),
                ContentType = "application/json"
            };

            serviceBusMessage.ApplicationProperties.Add("BoardId", boardId.ToString());
            serviceBusMessage.ApplicationProperties.Add("JobId", jobId.ToString());
            serviceBusMessage.ApplicationProperties.Add("JobType", "BoardDearchival");

            await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            _logger.LogInformation("Successfully sent dearchival message to Service Bus for board {BoardId} and job {JobId}", boardId, jobId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue dearchival message for board {BoardId} and job {JobId}", boardId, jobId);
            return false;
        }
    }

    public async Task<IEnumerable<ArchivalJobDto>> GetPendingDearchivalJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPendingDearchivalJobsQuery(), cancellationToken);
    }


    public async Task<ArchivalJobDto> MarkBoardAsArchivedAsync(Guid boardId, string? archivalReason = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking board {BoardId} as archived", boardId);

            await _mediator.Send(new UpdateBoardArchivalStatusCommand(
                BoardId: boardId,
                IsArchived: true,
                ArchivedAt: DateTime.UtcNow,
                ArchivalReason: archivalReason
            ), cancellationToken);

            var job = await _mediator.Send(new CreateArchivalJobCommand(
                BoardId: boardId,
                JobType: "BoardArchival",
                Metadata: archivalReason != null ? JsonSerializer.Serialize(new { ArchivalReason = archivalReason }) : null
            ), cancellationToken);

            _logger.LogInformation("Created archival job {JobId} for board {BoardId}", job.Id, boardId);

            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking board {BoardId} as archived", boardId);
            throw;
        }
    }


    public async Task ProcessArchivedBoardsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting to process archived boards");

            var pendingJobs = await _mediator.Send(new GetArchivalJobsForProcessingQuery(), cancellationToken);

            _logger.LogInformation("Found {Count} pending archival jobs", pendingJobs.Count());

            foreach (var job in pendingJobs)
            {
                try
                {
                    await _mediator.Send(new UpdateArchivalJobStatusCommand(
                        JobId: job.Id,
                        Status: (int)ArchivalStatus.InProgress,
                        ErrorMessage: null,
                        ProcessedBy: "ArchivalService"
                    ), cancellationToken);

                    var success = await EnqueueArchivedBoardAsync(job.BoardId, job.Id, cancellationToken);

                    if (success)
                    {
                        _logger.LogInformation("Successfully enqueued board {BoardId} with job {JobId}", job.BoardId, job.Id);
                    }
                    else
                    {
                        await _mediator.Send(new FailArchivalJobCommand(
                            JobId: job.Id,
                            ErrorMessage: "Failed to enqueue message to Service Bus",
                            ProcessedBy: "ArchivalService"
                        ), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing archival job {JobId}", job.Id);

                    await _mediator.Send(new FailArchivalJobCommand(
                        JobId: job.Id,
                        ErrorMessage: ex.Message,
                        ProcessedBy: "ArchivalService"
                    ), cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessArchivedBoardsAsync");
            throw;
        }
    }


    public async Task<bool> EnqueueArchivedBoardAsync(Guid boardId, Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            var archivalMessage = new ArchivalMessage
            {
                BoardId = boardId,
                JobId = jobId,
                JobType = "BoardArchival",
                EnqueuedAt = DateTime.UtcNow
            };

            var messageBody = JsonSerializer.Serialize(archivalMessage);
            var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = jobId.ToString(),
                ContentType = "application/json"
            };

            serviceBusMessage.ApplicationProperties.Add("BoardId", boardId.ToString());
            serviceBusMessage.ApplicationProperties.Add("JobId", jobId.ToString());
            serviceBusMessage.ApplicationProperties.Add("JobType", "BoardArchival");

            await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            _logger.LogInformation("Successfully sent message to Service Bus for board {BoardId} and job {JobId}", boardId, jobId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue message for board {BoardId} and job {JobId}", boardId, jobId);
            return false;
        }
    }


    public async Task<IEnumerable<ArchivalJobDto>> GetPendingArchivalJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPendingArchivalJobsQuery(), cancellationToken);
    }

    public async Task<bool> UpdateJobStatusAsync(Guid jobId, ArchivalStatus status, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new UpdateArchivalJobStatusCommand(
            JobId: jobId,
            Status: (int)status,
            ErrorMessage: errorMessage,
            ProcessedBy: "ArchivalService"
        ), cancellationToken);
    }

    public void Dispose()
    {
    }
}
