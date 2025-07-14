using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ArchivalJobRepository : Repository<ArchivalJob, Guid>, IArchivalJobRepository
{
    public ArchivalJobRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ArchivalJob>> GetPendingJobsAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(job => job.Status == ArchivalStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetJobsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(job => job.BoardId == boardId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetJobsByStatusAsync(
        ArchivalStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(job => job.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivalJob>> GetJobsForProcessingAsync(
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        if (createdBefore.HasValue)
        {
            query = query.Where(job => job.CreatedAt < createdBefore.Value);
        }
        return await query
            .Where(job => job.Status == ArchivalStatus.Pending || job.Status == ArchivalStatus.InProgress)
            .ToListAsync(cancellationToken);
    }

    public async Task<ArchivalJob?> GetActiveJobByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(job => job.BoardId == boardId && job.Status == ArchivalStatus.InProgress, cancellationToken);
    }

    public async Task<bool> UpdateJobStatusAsync(
        Guid jobId,
        ArchivalStatus status,
        string? errorMessage = null,
        string? processedBy = null,
        CancellationToken cancellationToken = default)
    {
        var job = await DbSet.FindAsync([jobId], cancellationToken);
        if (job == null) return false;

        job.Status = status;
        job.ErrorMessage = errorMessage;
        job.ProcessedBy = processedBy;
        return await Context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CompleteJobAsync(
        Guid jobId,
        string? blobPath = null,
        string? processedBy = null,
        CancellationToken cancellationToken = default)
    {
        var job = await DbSet.FindAsync([jobId], cancellationToken);
        if (job == null) return false;
        job.Status = ArchivalStatus.Completed;
        job.BlobPath = blobPath;
        job.ProcessedBy = processedBy;
        job.CompletedAt = DateTime.UtcNow;
        return await Context.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<bool> FailJobAsync(
        Guid jobId,
        string errorMessage,
        string? processedBy = null,
        CancellationToken cancellationToken = default)
    {
        var job = await DbSet.FindAsync([jobId], cancellationToken);
        if (job == null) return false;
        job.Status = ArchivalStatus.Failed;
        job.ErrorMessage = errorMessage;
        job.ProcessedBy = processedBy;
        return await Context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<ArchivalJob> CreateJobAsync(
        Guid boardId,
        string jobType = "BoardArchival",
        string? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var job = new ArchivalJob
        {
            BoardId = boardId,
            JobType = jobType,
            Metadata = metadata
        };
        await DbSet.AddAsync(job, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return job;
    }
}