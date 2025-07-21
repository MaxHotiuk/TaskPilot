using Domain.Dtos.Boards;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class ArchivalJobMappingExtensions
{
    public static ArchivalJobDto ToDto(this ArchivalJob job)
    {
        return new ArchivalJobDto
        {
            Id = job.Id,
            BoardId = job.BoardId,
            JobType = job.JobType,
            Metadata = job.Metadata,
            BlobPath = job.BlobPath,
            ProcessedBy = job.ProcessedBy,
            ErrorMessage = job.ErrorMessage,
            Status = (int)job.Status,
            CreatedAt = job.CreatedAt,
            ProcessedAt = job.ProcessedAt
        };
    }

    public static IEnumerable<ArchivalJobDto> ToDto(this IEnumerable<ArchivalJob> jobs)
    {
        return jobs.Select(ToDto);
    }
}
