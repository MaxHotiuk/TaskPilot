using Domain.Dtos.Boards;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.ArchivalJobs
{
    public class GetPendingDearchivalJobsQuery : IRequest<IEnumerable<ArchivalJobDto>>
    {
    }
}
