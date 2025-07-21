using Application.Abstractions.Persistence;
using Domain.Dtos.Boards;
using Application.Queries.ArchivalJobs;
using Domain.Enums;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetPendingDearchivalJobsQueryHandler : IRequestHandler<GetPendingDearchivalJobsQuery, IEnumerable<ArchivalJobDto>>
{
    private readonly ICosmosArchivalJobRepository _repository;

    public GetPendingDearchivalJobsQueryHandler(ICosmosArchivalJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetPendingDearchivalJobsQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _repository.GetPendingJobsAsync(cancellationToken);
        return jobs.Where(j => j.JobType == "BoardDearchival" && j.Status == ArchivalStatus.Pending)
            .Select(j => new ArchivalJobDto
            {
                Id = j.Id,
                BoardId = j.BoardId,
                JobType = j.JobType,
                Metadata = j.Metadata,
                BlobPath = j.BlobPath,
                ProcessedBy = j.ProcessedBy,
                ErrorMessage = j.ErrorMessage,
                Status = (int)j.Status,
                CreatedAt = j.CreatedAt,
                ProcessedAt = j.ProcessedAt
            }).ToList();
    }
}
