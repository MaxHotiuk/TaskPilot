using MediatR;

namespace Application.Commands.ArchivalJobs
{
    public record RemoveArchivalJobCommand(Guid JobId) : IRequest<bool>;
}
