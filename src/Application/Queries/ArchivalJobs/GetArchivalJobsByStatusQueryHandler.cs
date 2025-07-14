using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetArchivalJobsByStatusQueryHandler : BaseQueryHandler, IRequestHandler<GetArchivalJobsByStatusQuery, IEnumerable<ArchivalJobDto>>
{
    public GetArchivalJobsByStatusQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetArchivalJobsByStatusQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var jobs = await unitOfWork.ArchivalJobs.GetJobsByStatusAsync((Domain.Entities.ArchivalStatus)request.Status, cancellationToken);
            return jobs.ToDto();
        }, cancellationToken);
    }
}
