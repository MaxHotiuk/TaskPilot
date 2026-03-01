using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Dtos.Organizations;
using MediatR;

namespace Application.Queries.Organizations;

public record GetOrganizationMembersQuery(Guid OrganizationId, Guid RequesterId) : IQuery<IEnumerable<OrganizationMemberDto>>;

public class GetOrganizationMembersQueryHandler : BaseQueryHandler, IRequestHandler<GetOrganizationMembersQuery, IEnumerable<OrganizationMemberDto>>
{
    private readonly IOrganizationMemberRepository _organizationMemberRepository;

    public GetOrganizationMembersQueryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationMemberRepository organizationMemberRepository) 
        : base(unitOfWorkFactory)
    {
        _organizationMemberRepository = organizationMemberRepository;
    }

    public async Task<IEnumerable<OrganizationMemberDto>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        // Verify requester is member of organization
        var isMember = await _organizationMemberRepository.IsMemberOfOrganizationAsync(
            request.OrganizationId, 
            request.RequesterId, 
            cancellationToken);

        if (!isMember)
            throw new ValidationException("You must be a member of this organization to view its members");

        var members = await _organizationMemberRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);

        return members.Select(m => new OrganizationMemberDto
        {
            UserId = m.UserId,
            Username = m.User.Username,
            Email = m.User.Email,
            Role = m.Role.ToString(),
            IsInvited = m.IsInvited,
            JoinedAt = m.CreatedAt
        }).OrderBy(m => m.Role).ThenBy(m => m.Username);
    }
}
