using Application.Common.Dtos.Boards;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.ArchivalJobs
{
    public class GetDearchivalJobsForProcessingQuery : IRequest<IEnumerable<ArchivalJobDto>>
    {
    }
}
