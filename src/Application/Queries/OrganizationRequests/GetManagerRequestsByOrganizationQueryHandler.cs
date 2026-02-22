using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Dtos.OrganizationRequests;
using MediatR;

namespace Application.Queries.OrganizationRequests;

public record GetManagerRequestsByOrganizationQuery(Guid OrganizationId) : IQuery<IEnumerable<ManagerRequestDto>>;

public class GetManagerRequestsByOrganizationQueryHandler : BaseQueryHandler, IRequestHandler<GetManagerRequestsByOrganizationQuery, IEnumerable<ManagerRequestDto>>
{
    private readonly IOrganizationManagerRequestRepository _requestRepository;

    public GetManagerRequestsByOrganizationQueryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationManagerRequestRepository requestRepository) 
        : base(unitOfWorkFactory)
    {
        _requestRepository = requestRepository;
    }

    public async Task<IEnumerable<ManagerRequestDto>> Handle(GetManagerRequestsByOrganizationQuery request, CancellationToken cancellationToken)
    {
        var requests = await _requestRepository.GetRequestsByOrganizationAsync(request.OrganizationId, cancellationToken);

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
