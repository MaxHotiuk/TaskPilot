using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetArchivalJobsForProcessingQueryHandler : BaseQueryHandler, IRequestHandler<GetArchivalJobsForProcessingQuery, IEnumerable<ArchivalJobDto>>
{
    public GetArchivalJobsForProcessingQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetArchivalJobsForProcessingQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var jobs = await unitOfWork.ArchivalJobs.GetJobsForProcessingAsync(request.CreatedBefore, cancellationToken);
            return jobs.ToDto();
        }, cancellationToken);
    }
}
