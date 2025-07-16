using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Cosmos;

public class CosmosArchivalJobRepository : ICosmosArchivalJobRepository
{
    private readonly CosmosDbContext _dbContext;

    public CosmosArchivalJobRepository(CosmosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ArchivalJob> CreateJobAsync(Guid boardId, string jobType, string? metadata, CancellationToken cancellationToken = default)
    {
        var job = new ArchivalJob
        {
            Id = Guid.NewGuid(),
            BoardId = boardId,
            JobType = jobType,
            Metadata = metadata,
            Status = ArchivalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.ArchivalJobs.AddAsync(job, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<bool> CompleteJobAsync(Guid jobId, string? blobPath = null, string? processedBy = null, CancellationToken cancellationToken = default)
    {
        var job = await _dbContext.ArchivalJobs.FindAsync(new object[] { jobId }, cancellationToken);

        if (job == null)
        {
            return false;
        }

        job.Status = ArchivalStatus.Completed;
        job.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(blobPath))
        {
            job.Metadata = blobPath;
        }

        if (!string.IsNullOrEmpty(processedBy))
        {
            job.ProcessedBy = processedBy;
        }

        _dbContext.ArchivalJobs.Update(job);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> FailJobAsync(Guid jobId, string errorMessage, string? processedBy = null, CancellationToken cancellationToken = default)
    {
        var job = await _dbContext.ArchivalJobs.FindAsync(new object[] { jobId }, cancellationToken);

        if (job == null)
        {
            return false;
        }

        job.Status = ArchivalStatus.Failed;
        job.UpdatedAt = DateTime.UtcNow;
        job.ErrorMessage = errorMessage;

        if (!string.IsNullOrEmpty(processedBy))
        {
            job.ProcessedBy = processedBy;
        }

        _dbContext.ArchivalJobs.Update(job);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateJobStatusAsync(Guid jobId, ArchivalStatus status, string? errorMessage = null, string? processedBy = null, CancellationToken cancellationToken = default)
    {
        var job = await _dbContext.ArchivalJobs.FindAsync(new object[] { jobId }, cancellationToken);

        if (job == null)
        {
            return false;
        }

        job.Status = status;
        job.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            job.ErrorMessage = errorMessage;
        }

        if (!string.IsNullOrEmpty(processedBy))
        {
            job.ProcessedBy = processedBy;
        }

        _dbContext.ArchivalJobs.Update(job);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<ArchivalJob>> GetPendingJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ArchivalJobs
            .Where(j => j.Status == ArchivalStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetJobsByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ArchivalJobs
            .Where(j => j.BoardId == boardId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetJobsByStatusAsync(ArchivalStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ArchivalJobs
            .Where(j => j.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetJobsForProcessingAsync(DateTime? createdBefore = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ArchivalJobs.Where(j => j.Status == ArchivalStatus.Pending);

        if (createdBefore.HasValue)
        {
            query = query.Where(j => j.CreatedAt < createdBefore.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ArchivalJob?> GetActiveJobByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ArchivalJobs
            .Where(j => j.BoardId == boardId && j.Status == ArchivalStatus.Pending)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ArchivalJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ArchivalJobs.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ArchivalJobs.ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ArchivalJob job, CancellationToken cancellationToken = default)
    {
        _dbContext.ArchivalJobs.Update(job);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid boardId, CancellationToken cancellationToken = default)
    {
        var job = await _dbContext.ArchivalJobs.FindAsync(new object[] { id }, cancellationToken);

        if (job != null)
        {
            _dbContext.ArchivalJobs.Remove(job);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}