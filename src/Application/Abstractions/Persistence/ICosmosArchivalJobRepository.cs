using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ICosmosArchivalJobRepository
{
    Task<bool> CompleteJobAsync(
        Guid jobId,
        string? blobPath = null,
        string? processedBy = null,
        CancellationToken cancellationToken = default);
    Task<bool> FailJobAsync(
        Guid jobId,
        string errorMessage,
        string? processedBy = null,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateJobStatusAsync(
        Guid jobId,
        ArchivalStatus status,
        string? errorMessage = null,
        string? processedBy = null,
        CancellationToken cancellationToken = default);

    Task<ArchivalJob> CreateJobAsync(
        Guid boardId,
        string jobType,
        string? metadata,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ArchivalJob>> GetPendingJobsAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ArchivalJob>> GetJobsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ArchivalJob>> GetJobsByStatusAsync(
        ArchivalStatus status,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ArchivalJob>> GetJobsForProcessingAsync(
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default);

    Task<ArchivalJob?> GetActiveJobByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<ArchivalJob?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ArchivalJob>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ArchivalJob job,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid boardId,
        CancellationToken cancellationToken = default);
}