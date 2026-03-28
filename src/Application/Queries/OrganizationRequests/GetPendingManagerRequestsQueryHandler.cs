using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Dtos.OrganizationRequests;
using MediatR;

namespace Application.Queries.OrganizationRequests;

public record GetPendingManagerRequestsQuery : IQuery<IEnumerable<ManagerRequestDto>>;

public class GetPendingManagerRequestsQueryHandler : BaseQueryHandler, IRequestHandler<GetPendingManagerRequestsQuery, IEnumerable<ManagerRequestDto>>
{
    private readonly IOrganizationManagerRequestRepository _requestRepository;

    public GetPendingManagerRequestsQueryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationManagerRequestRepository requestRepository) 
        : base(unitOfWorkFactory)
    {
        _requestRepository = requestRepository;
    }

    public async Task<IEnumerable<ManagerRequestDto>> Handle(GetPendingManagerRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await _requestRepository.GetPendingRequestsAsync(cancellationToken);

        return requests.Select(r => new ManagerRequestDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User.Username,
            UserEmail = r.User.Email,
            OrganizationId = r.OrganizationId,
            OrganizationName = r.Organization.Name,
            Message = r.Message,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            ReviewedAt = r.ReviewedAt,
            ReviewerName = r.Reviewer?.Username,
            ReviewNotes = r.ReviewNotes
        });
    }
}
