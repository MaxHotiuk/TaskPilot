using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Enums;
using MediatR;

namespace Application.Commands.OrganizationMembers;

public record DemoteFromManagerCommand(Guid OrganizationId, Guid UserId, Guid DemoterId) : IRequest<Unit>;

public class DemoteFromManagerCommandHandler : BaseCommandHandler, IRequestHandler<DemoteFromManagerCommand, Unit>
{
    private readonly IOrganizationMemberRepository _organizationMemberRepository;

    public DemoteFromManagerCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationMemberRepository organizationMemberRepository) 
        : base(unitOfWorkFactory)
    {
        _organizationMemberRepository = organizationMemberRepository;
    }

    public async Task<Unit> Handle(DemoteFromManagerCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            // Verify demoter is a manager or admin
            var isDemoterManager = await _organizationMemberRepository.IsManagerOfOrganizationAsync(
                request.OrganizationId, 
                request.DemoterId, 
                cancellationToken);

            if (!isDemoterManager)
            {
                var demoter = await unitOfWork.Users.GetByIdAsync(request.DemoterId, cancellationToken);
                if (demoter == null || demoter.Role != Domain.Common.Authorization.Roles.Admin)
                    throw new ValidationException("Only managers or administrators can demote managers");
            }

            // Get member to demote
            var member = await _organizationMemberRepository.GetOrganizationMemberAsync(
                request.OrganizationId, 
                request.UserId, 
                cancellationToken);

            if (member == null)
                throw new NotFoundException("Organization member not found");

            if (member.Role != OrganizationMemberRole.Manager)
                throw new ValidationException("User is not a manager");

            // Check if this is the last manager
            var managers = await _organizationMemberRepository.GetManagersByOrganizationIdAsync(
                request.OrganizationId, 
                cancellationToken);

            if (managers.Count() == 1)
                throw new ValidationException("Cannot demote the last manager. Promote another member first.");

            // Demote to member
            member.Role = OrganizationMemberRole.Member;
            member.UpdatedAt = DateTime.UtcNow;

            return Unit.Value;
        }, cancellationToken);
    }
}
